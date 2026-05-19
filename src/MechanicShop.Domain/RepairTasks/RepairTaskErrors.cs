using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks
{
    public static class RepairTaskErrors
    {
        public static Error NameRequired
            => Error.Validation("RepairTask.Name.Required", "Name is required");
        public static Error LaborCostInvalid =>
        Error.Validation("RepairTask.LaborCost.Invalid", "Labor cost must be between 1 and 10,000.");
        public static Error DurationInvalid =>
        Error.Validation("RepairTask.Duration.Invalid", "Invalid duration selected.");
        public static Error DuplicateName =>
            Error.Conflict("RepairTaskPart.Duplicate", "A part with the same name already exists in this repair task.");
    }
}
