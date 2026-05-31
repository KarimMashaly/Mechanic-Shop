using MechanicShop.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.WorkOrders
{
    public class UpdateWorkOrderStateRequest
    {
        [Required(ErrorMessage = "New state is required.")]
        [Range(0, 3, ErrorMessage = "Invalid range [0, 1, 2 or 3]")]
        public WorkOrderState NewState { get; set; }
    }
}
