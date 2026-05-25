using FluentValidation;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks
{
    public sealed class UpdateWorkOrderRepairTasksCommandHandler(
        IAppDbContext context,
        ILogger<UpdateWorkOrderRepairTasksCommandHandler> logger,
        HybridCache cache, 
        IWorkOrderPolicy workOrderPolicy)
        : IRequestHandler<UpdateWorkOrderRepairTasksCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateWorkOrderRepairTasksCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;

        public async Task<Result<Updated>> Handle(UpdateWorkOrderRepairTasksCommand command, CancellationToken ct)
        {
            var workOrder = await _context.WorkOrders
                .Include(w => w.RepairTasks)
                .FirstOrDefaultAsync(wo => wo.Id == command.workOrderId, ct);

            if(workOrder is null)
            {
                logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", command.workOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            if(command.RepairTaskIds.Length == 0)
            {
                logger.LogError("Empty RepairTaskIds list submitted.");

                return RepairTaskErrors.AtLeastOneRepairTaskIsRequired;
            }

            var requestedTasks = await _context.RepairTasks
                .Where(rt => command.RepairTaskIds.Contains(rt.Id))
                .ToListAsync(ct);

            if(requestedTasks.Count != command.RepairTaskIds.Count())
            {
                var missingIds = command.RepairTaskIds.Except(requestedTasks.Select(r => r.Id)).ToArray();

                _logger.LogError("One or more RepairTasks not found. {ids}", string.Join(", ", missingIds));

                return ApplicationErrors.RepairTaskNotFound;
            }

            var clearExistingResult = workOrder.ClearRepairTasks();

            if (clearExistingResult.IsError)
                return clearExistingResult.Errors!;

            foreach(var repairTask in requestedTasks)
            {
                var addRepairTaskResult = workOrder.AddRepairTask(repairTask);

                if(addRepairTaskResult.IsError)
                    return addRepairTaskResult.Errors!;
            }

            var totalDuration = TimeSpan.FromMinutes(requestedTasks.Sum(t => (int)t.EstimatedDurationInMins));

            var newEndAt = workOrder.StartAtUtc + totalDuration;

            if (_workOrderPolicy.IsOutsideOperatingHours(workOrder.StartAtUtc, totalDuration))
                return Error.Conflict("WorkOrder_Outside_OperatingHours", "WorkOrder timing exceeds business hours.");

            var checkSpotResult = await _workOrderPolicy.CheckSpotAvailabilityAsync(
                workOrder.Spot,
                workOrder.StartAtUtc,
                newEndAt,
                excludeWorkOrderId: workOrder.Id,
                ct: ct);

            if(checkSpotResult.IsError)
                return checkSpotResult.Errors!;

            if(await _workOrderPolicy.IsLaborOccupied(workOrder.LaborId, workOrder.Id, workOrder.StartAtUtc, newEndAt))
                return ApplicationErrors.LaborOccupied;

            workOrder.UpdateTiming(workOrder.StartAtUtc, newEndAt);

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await _context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("work-order", ct);

            return Result.Updated;
        }
    }
}
