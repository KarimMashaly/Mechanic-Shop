using MechanicShop.Domain.Common;

namespace MechanicShop.Domain.WorkOrders.Events
{
    public class WorkOrderEvent : DomainEvent
    {
        public Guid WorkOrderId { get; set; }
    }
}
