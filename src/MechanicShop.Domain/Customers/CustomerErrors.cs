using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers
{
    public static class CustomerErrors
    {
        public static Error NameRequired =>
            Error.Validation("Customer_Name_Required", "Customer name is required");
        public static Error PhoneRequired =>
            Error.Validation("Customer_Phone_Required", "Customer phone is required");
        public static Error EmailRequired =>
            Error.Validation("Customer_Email_Required", "Customer email is required");
        public static Error EmailInvalid =>
            Error.Validation("Customer_Email_Invalid", "Customer email is invalid");
        public static Error CustomerExists =>
        Error.Conflict("Customer_Email_Exists", "A customer with this email already exists.");
        public static readonly Error InValidPhoneNumber =
            Error.Conflict("Customer.InvalidPhoneNumber", "Phone number must be 7–15 digits and may start with '+'.");
        public static readonly Error CannotDeleteCustomerWithWorkOrders =
        Error.Conflict("Customer.CannotDelete", "Customer cannot be deleted due to existing work orders.");
    }
}
