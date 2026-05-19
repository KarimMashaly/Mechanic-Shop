using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers.Vehicles
{
    public static class VehicleErrors
    {
        public static Error MakeRequired =>
            Error.Validation("Vehicle_Make_Required", "Vehicel make is required");
        public static Error ModelRequired =>
            Error.Validation("Vehicle_Model_Required", "Vehicel model is required");
        public static Error LicensePlateRequired =>
            Error.Validation("Vehicle_LicensePlate_Required", "Vehicel LicensePlate is required");
        public static Error YearInvalid =>
            Error.Validation("Vehicle_Year_Invalid", "Vehicel year is invalid");
    }
}
