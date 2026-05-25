using MechanicShop.Domain.Common;

namespace MechanicShop.Domain.WorkOrders.Events
{
    public class WorkOrderCompleted : DomainEvent
    {
        public Guid WorkOrderId { get; set; }
    }
}
