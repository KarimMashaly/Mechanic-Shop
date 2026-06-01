using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers.Vehicles
{
    public class Vehicle : AuditableEntity
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? LicensePlate { get; set; }
        public Guid CustomerId { get; private set; }
        public Customer? Customer { get; set; }

        public string VehicleInfo => $"{Make} | {Model} | {Year}";

        private Vehicle() { }

        private Vehicle(Guid id, string make, string model, int year, string licensePlate)
            : base(id)
        {
            Make = make;
            Model = model;
            Year = year;
            LicensePlate = licensePlate;
        }

        public static Result<Vehicle> Create(Guid id, string make, string model, int year, string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(make))
                return VehicleErrors.MakeRequired;

            if (string.IsNullOrWhiteSpace(model))
                return VehicleErrors.ModelRequired;

            if (string.IsNullOrWhiteSpace(licensePlate))
                return VehicleErrors.LicensePlateRequired;

            if (year < 1886 || year > DateTime.UtcNow.Year)
                return VehicleErrors.YearInvalid;

            return new Vehicle(id, make, model, year, licensePlate);
        }

        public Result<Updated>Update(string make, string model, int year, string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(make))
                return VehicleErrors.MakeRequired;

            if (string.IsNullOrWhiteSpace(model))
                return VehicleErrors.ModelRequired;

            if (string.IsNullOrWhiteSpace(licensePlate))
                return VehicleErrors.LicensePlateRequired;

            if (year < 1886 || year > DateTime.UtcNow.Year)
                return VehicleErrors.YearInvalid;

            Make = make;
            Model = model;
            Year = year;
            LicensePlate = licensePlate;

            return Result.Updated;
        }
    }
}
