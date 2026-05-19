using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;

using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MechanicShop.Domain.Customers
{
    public sealed class Customer : AuditableEntity
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        private readonly List<Vehicle> _vehicles = [];

        public IEnumerable<Vehicle> Vehicles => _vehicles.AsReadOnly();

        private Customer() { }

        private Customer(Guid id, string name, string phone, string email, List<Vehicle>vehicles)
            : base(id)
        {
            Name = name;
            Phone = phone;
            Email = email;
            _vehicles = vehicles;
        }

        public static Result<Customer>Create(Guid id, string name, string phoneNumber, string email, List<Vehicle>vehicles)
        {
            if (string.IsNullOrWhiteSpace(name))
                return CustomerError.NameRequired;

            if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
                return CustomerError.InValidPhoneNumber;

            if(string.IsNullOrWhiteSpace(email))
                return CustomerError.EmailRequired;

            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                return CustomerError.EmailInvalid;
            }

            return new Customer(id, name, phoneNumber, email, vehicles);
        }

        public Result<Updated> Update(string name, string phone, string email)
        {
            if(string.IsNullOrWhiteSpace(name))
                return CustomerError.NameRequired;

            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\+?\d{7,15}$"))
                return CustomerError.InValidPhoneNumber;

            if (string.IsNullOrWhiteSpace(email))
                return CustomerError.EmailRequired;

            Name = name;
            Phone = phone;
            Email = email;

            return Result.Updated;
        }

        public async Task<Result<Updated>> UpsertParts(List<Vehicle> incomingVehicles)
        {
            _vehicles.RemoveAll(existing => incomingVehicles.All(v => v.Id != existing.Id));

            foreach (var incoming in incomingVehicles)
            {
                var exsiting = _vehicles.FirstOrDefault(v => v.Id == incoming.Id);

                if(exsiting is null)
                    _vehicles.Add(incoming);
                else
                {
                    var updateVehicleResult = exsiting.Update(incoming.Make!, incoming.Model!, incoming.Year, incoming.LicensePlate!);

                    if (updateVehicleResult.IsError)
                        return updateVehicleResult.Errors!;
                }
            }
            return Result.Updated;
        }
    }

}
