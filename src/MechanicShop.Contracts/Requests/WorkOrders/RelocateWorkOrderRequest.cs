using MechanicShop.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.WorkOrders
{
    public class RelocateWorkOrderRequest
    {
        [Required(ErrorMessage = "New Start is required.")]
        public DateTimeOffset NewStartAtUtc { get; set; }

        [Required(ErrorMessage = "New Spot is required.")]
        [Range(0, 3, ErrorMessage = "Invalid range [0, 1, 2 or 3]")]
        public Spot NewSpot { get; set; }
    }
}
