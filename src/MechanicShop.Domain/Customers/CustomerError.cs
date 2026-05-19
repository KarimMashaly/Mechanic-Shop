using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers
{
    public static class CustomerError
    {
        public static Error NameRequired =>
            Error.Validation("Customer_Name_Required", "Customer name is required");
        public static Error PhoneRequired =>
            Error.Validation("Customer_Phone_Required", "Customer phone is required");
        public static Error EmailRequired =>
            Error.Validation("Customer_Email_Required", "Customer email is required");
        public static Error EmailInvalid =>
            Error.Validation("Customer_Email_Invalid", "Customer email is invalid");



        public static readonly Error InValidPhoneNumber =
            Error.Conflict("Customer.InvalidPhoneNumber", "Phone number must be 7–15 digits and may start with '+'.");
    }
}
