using MechanciShop.Infrustructure.Settings;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MechanciShop.Infrustructure.Services
{
    public class WorkOrderPolicy(IAppDbContext context, IOptions<AppSettings>options) : IWorkOrderPolicy
    {
        private readonly IAppDbContext _context = context;
        private readonly AppSettings _appSettings = options.Value;

        public async Task<Result<Success>> CheckSpotAvailabilityAsync(Spot spot, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludeWorkOrderId = null, CancellationToken ct = default)
        {
            var isOccupied = await _context.WorkOrders
                .AnyAsync(wo =>
                wo.Spot == spot &&
                wo.StartAtUtc < endAt &&
                wo.EndAtUtc > startAt &&
                (!excludeWorkOrderId.HasValue || wo.Id != excludeWorkOrderId), ct);

            return isOccupied
             ? Error.Conflict("MechanicShop_Spot_Full", "The selected time slot is unavailable for the requested services.")
             : Result.Success;
        }

        public async Task<bool> IsLaborOccupied(Guid laborId, Guid WorkOrderId, DateTimeOffset startAt, DateTimeOffset endAt)
        {
            return await _context.WorkOrders
                .AnyAsync(wo =>
                wo.LaborId == laborId &&
                wo.Id != WorkOrderId &&
                wo.StartAtUtc < endAt &&
                wo.EndAtUtc > startAt);
        }

        public bool IsOutsideOperatingHours(DateTimeOffset startAt, TimeSpan Duration)
        {
            var opening = startAt.Date.Add(_appSettings.OpeningTime.ToTimeSpan());
            var closing = startAt.Date.Add(_appSettings.ClosingTime.ToTimeSpan());
            var endAt = startAt + Duration;

            return startAt < opening || endAt > closing;
        }

        public async Task<bool> IsVehicleAlreadyScheduled(Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludedWorkOrderId = null)
        {
            return await _context.WorkOrders.AnyAsync(a =>
            a.VehicleId == vehicleId &&
            (excludedWorkOrderId == null || a.Id != excludedWorkOrderId) &&
            a.StartAtUtc < endAt &&
            a.EndAtUtc > startAt);
        }

        public Result<Success> ValidateMinimumRequirement(DateTimeOffset startAt, DateTimeOffset endAt)
        {
            if ((endAt - startAt) < TimeSpan.FromMinutes(_appSettings.MinimumAppointmentDurationInMinutes))
            {
                return Error.Conflict(
                "WorkOrder_TooShort",
                $"WorkOrder duration must be at least {_appSettings.MinimumAppointmentDurationInMinutes} minutes.");
            }

            return Result.Success;
        }
    }
}
