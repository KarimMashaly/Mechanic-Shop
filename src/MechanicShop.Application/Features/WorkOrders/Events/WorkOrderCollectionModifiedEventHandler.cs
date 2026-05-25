using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Events
{
    public sealed class WorkOrderCollectionModifiedEventHandler(IWorkOrderNotifier notifier)
        : INotificationHandler<WorkOrderCollectionModified>
    {
        public Task Handle(WorkOrderCollectionModified notification, CancellationToken ct) =>
            notifier.NotifyWorkOrdersChangedAsync(ct);

    }
}
