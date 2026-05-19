using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity;

namespace MechanicShop.Domain.Employees
{
    public class Employee : AuditableEntity
    {
        public string? FirstName { get; private set; } 
        public string? LastName { get; private set; }
        public Role Role{ get; private set; }
        public string FullName => $"{FirstName} {LastName}";

        private Employee() { }

        private Employee(Guid id, string firstName, string lastName, Role role)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }

        public static Result<Employee> Create(Guid id, string firstName, string lastName, Role role)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return EmployeeErrors.FirstNameRequired;

            if (string.IsNullOrWhiteSpace(lastName))
                return EmployeeErrors.LastNameRequired;

            if (!Enum.IsDefined(role))
            {
                return EmployeeErrors.RoleInvalid;
            }

            return new Employee(id, firstName, lastName, role);
        }
    }
}
