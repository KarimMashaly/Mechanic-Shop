using Microsoft.AspNetCore.SignalR;

namespace MechanciShop.Infrustructure.RealTime
{
    public class WorkOrderHub : Hub
    {
        public const string HubUrl = "/hubs/workorders";
    }
}
