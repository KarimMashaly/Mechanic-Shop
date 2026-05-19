using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Errors
{
    public static class ApplicationErrors
    {
        public static Error WorkOrderOutsideOperatingHour(DateTimeOffset startAtUtc, DateTimeOffset endAtUtc)
            => Error.Conflict("ApplicationErrors.WorkOrder.Outside.OperatingHour",
                $"The WorkOrder time ({startAtUtc} ? {endAtUtc}) is outside of operating hours.");


        public static Error CustomerNotFound
            => Error.NotFound(
                  "ApplicationErrors.Customer.NotFound",
                  "Customer does not exist.");

        public static Error RepairTaskNotFound
            => Error.NotFound(
                "RepairTask.NotFound",
                "Repair task does not exist.");
    }
}
