using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState
{
    public sealed class UpdateWorkOrderStateCommandHandler(
        IAppDbContext context,
        ILogger<UpdateWorkOrderStateCommandHandler> logger,
        HybridCache cache,
        TimeProvider datetime)
        : IRequestHandler<UpdateWorkOrderStateCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateWorkOrderStateCommand command, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                .FirstOrDefaultAsync(wo => wo.Id == command.WorkOrderId);

            if(workOrder is null)
            {
                logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", command.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            if(workOrder.StartAtUtc > datetime.GetUtcNow())
            {
                logger.LogError("State transition for WorkOrder Id '{WorkOrderId}` is not allowed before the work orders scheduled start time.", command.WorkOrderId);

                return WorkOrderErrors.StateTransitionNotAllowed(workOrder.StartAtUtc);
            }

            var updateStatusResult = workOrder.UpdateState(command.newState);

            if (updateStatusResult.IsError)
            {
                logger.LogError("Failed to update status: {Error}", updateStatusResult.TopError.Description);

                return updateStatusResult.Errors!;
            }

            if(command.newState is WorkOrderState.Completed)
            {
                workOrder.AddDomainEvent(new WorkOrderCompleted { WorkOrderId = command.WorkOrderId });
            }

            await context.SaveChangesAsync(ct);

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await cache.RemoveByTagAsync("work-order", ct);

            return Result.Updated;
        }
    }
}
