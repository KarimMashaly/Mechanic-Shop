using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.Customers
{
    public class CreateVehicleRequest
    {
        [Required(ErrorMessage = "Make is required.")]
        public string Make { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required.")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "LicensePlate is required.")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required.")]
        public int Year { get; set; }
    }
}
