namespace MechanicShop.Contracts.Requests.WorkOrders
{
    public class ModifyRepairTasksRequest
    {
        public Guid[] RepairTaskIds { get; set; } = [];
    }
}
