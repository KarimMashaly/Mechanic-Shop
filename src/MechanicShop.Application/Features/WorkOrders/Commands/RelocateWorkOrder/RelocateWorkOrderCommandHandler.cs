using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder
{
    public sealed class RelocateWorkOrderCommandHandler(
        IAppDbContext context,
        ILogger<RelocateWorkOrderCommandHandler> logger,
        HybridCache cache,
        IWorkOrderPolicy workOrderValidtor)
        : IRequestHandler<RelocateWorkOrderCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<RelocateWorkOrderCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private readonly IWorkOrderPolicy _appointmentValidator = workOrderValidtor;

        public async Task<Result<Updated>> Handle(RelocateWorkOrderCommand command, CancellationToken ct)
        {
            var workOrder = await _context.WorkOrders
                .Include(w => w.Labor)
                .Include(w => w.Vehicle)
                .Include(w => w.RepairTasks)
                .FirstOrDefaultAsync(wo => wo.Id == command.WorkOrderId, ct);

            if(workOrder is null)
            {
                _logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", command.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            var duration = workOrder.EndAtUtc.Subtract(workOrder.StartAtUtc).Duration();

            var newEndAt = command.NewStartAt.Add(duration);

            var checkSpotAvailabilityResult = await _appointmentValidator.CheckSpotAvailabilityAsync(
                command.NewSpot,
                command.NewStartAt,
                newEndAt,
                excludeWorkOrderId: command.WorkOrderId,
                ct: ct);

            if (checkSpotAvailabilityResult.IsError)
                return checkSpotAvailabilityResult.Errors!;

            if (await _appointmentValidator.IsLaborOccupied(workOrder.LaborId, command.WorkOrderId, command.NewStartAt, newEndAt))
            {
                _logger.LogError("Labor with Id '{LaborId}' is already occupied during the requested time.", workOrder.LaborId);

                return ApplicationErrors.LaborOccupied;
            }

            if (await _appointmentValidator.IsVehicleAlreadyScheduled(workOrder.VehicleId, command.NewStartAt, newEndAt, workOrder.Id))
            {
                _logger.LogError("Vehicle with Id '{VehicleId}' already has an overlapping WorkOrder.", workOrder.VehicleId);

                return ApplicationErrors.VehicleSchedulingConflict;
            }

            var updateTimingResult = workOrder.UpdateTiming(command.NewStartAt, newEndAt);
            
            if(updateTimingResult.IsError)
            {
                _logger.LogError("Failed to update timing: {Error}", updateTimingResult.TopError.Description);

                return updateTimingResult.Errors!;
            }

            var updateSpotResult = workOrder.UpdateSpot(command.NewSpot);

            if(updateSpotResult.IsError)
            {
                _logger.LogError("Failed to update Spot: {Error}", updateSpotResult.TopError.Description);

                return updateTimingResult.Errors!;
            }

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await _context.SaveChangesAsync(ct);

            await _cache.RemoveByTagAsync("work-order", ct);

            return Result.Updated;
        }
    }
}
