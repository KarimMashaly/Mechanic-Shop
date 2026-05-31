using MechanicShop.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.WorkOrders
{
    public class CreateWorkOrderRequest
    {
        [Required(ErrorMessage = "Labor is required.")]
        public Guid LaborId { get; set; }

        [Required(ErrorMessage = "Vehicle is required.")]
        public Guid VehicleId { get; set; }

        [Required(ErrorMessage = "StartAtUtc is required.")]
        public DateTimeOffset StartAtUtc { get; set; }

        [Required(ErrorMessage = "Spot is required.")]
        [Range(0, 3, ErrorMessage = "Invalid range [0, 1, 2 or 3]")]
        public Spot Spot { get; set; }

        [MinLength(1, ErrorMessage = "At least one repairt task must be selected.")]
        public List<Guid> RepairTaskIds { get; set; } = [];
    }
}
