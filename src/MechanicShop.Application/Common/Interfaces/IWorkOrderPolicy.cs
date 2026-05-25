using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Application.Common.Interfaces
{
    public interface IWorkOrderPolicy
    {
        bool IsOutsideOperatingHours(DateTimeOffset startAt, TimeSpan Duration);
        Result<Success> ValidateMinimumRequirement(DateTimeOffset startAt, DateTimeOffset endAt);
        Task<Result<Success>> CheckSpotAvailabilityAsync(Spot spot, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludeWorkOrderId = null, CancellationToken ct = default);
        Task<bool> IsLaborOccupied(Guid laborId, Guid WorkOrderId, DateTimeOffset startAt, DateTimeOffset endAt);
        Task<bool> IsVehicleAlreadyScheduled(Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludedWorkOrderId = null);
    }
}
