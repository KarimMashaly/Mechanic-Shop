using Microsoft.AspNetCore.SignalR;

namespace MechanicShop.Infrustructure.RealTime
{
    public class WorkOrderHub : Hub
    {
        public const string HubUrl = "/hubs/workorders";
    }
}
