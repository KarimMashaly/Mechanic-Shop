using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.Scheduling.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Scheduling.Queries.GetDailySchedule
{
    public class GetDailyScheduleQueryHandler(
        IAppDbContext context,
        TimeProvider dateTime)
        : IRequestHandler<GetDailyScheduleQuery, Result<ScheduleDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly TimeProvider _dateTime = dateTime;

        public async Task<Result<ScheduleDto>> Handle(GetDailyScheduleQuery query, CancellationToken ct)
        {
            var localStart = query.ScheduleDate.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, query.TimeZone);
            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, query.TimeZone);

            var workOrders = await _context.WorkOrders
                .Include(wo => wo.Vehicle)
                .Include(wo => wo.RepairTasks)
                .Include(wo => wo.Labor)
                .Where(wo =>
                    wo.StartAtUtc < utcEnd &&
                    wo.EndAtUtc > utcStart &&
                    (query.LaborId == null || wo.LaborId == query.LaborId))
                .ToListAsync(ct);

            var now = TimeZoneInfo.ConvertTime(_dateTime.GetUtcNow(), query.TimeZone);

            var result = new ScheduleDto
            {
                OnDate = query.ScheduleDate,
                EndOfDay = localEnd < now,
                Spots = []
            };

            foreach(var spot in Enum.GetValues<Spot>())
            {
                var current = localStart;
                var slots = new List<AvailabilitySlotDto>();

                var woBySpot = workOrders
                    .Where(wo => wo.Spot == spot)
                    .OrderBy(wo => wo.StartAtUtc)
                    .ToList();

                while(current < localEnd)
                {
                    var next = current.AddMinutes(15);
                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(current, query.TimeZone);
                    var endUtc = TimeZoneInfo.ConvertTimeToUtc(next, query.TimeZone);

                    var wo = woBySpot.FirstOrDefault(w =>
                        w.StartAtUtc < endUtc && w.EndAtUtc > startUtc);

                    if(wo is not null)
                    {
                        if(!slots.Any(s => s.WorkOrderId == wo.Id))
                        {
                            slots.Add(new AvailabilitySlotDto
                            {
                                WorkOrderId = wo.Id,
                                Spot = spot,
                                StartAt = wo.StartAtUtc,
                                EndAt = wo.EndAtUtc,
                                State = wo.State,
                                Vehicle = FormateVehicle(wo.Vehicle!),
                                IsAvailable = false,
                                IsOccupied = true,
                                RepairTasks = [..wo.RepairTasks.ToList().ConvertAll(rt => rt.ToDto())],
                                Labor = wo.Labor!.ToDto(),
                                WorkOrderLocked = !wo.IsEditable
                            });
                        }
                    }
                    else
                    {
                        slots.Add(new AvailabilitySlotDto
                        {
                            Spot = spot,
                            StartAt = startUtc,
                            EndAt = endUtc,
                            IsAvailable = current >= now,
                            WorkOrderLocked = false
                        });
                    }

                    current = next;
                }

                result.Spots.Add(new SpotDto
                {
                    Spot = spot,
                    Slots = slots
                });
            }

            return result;
        }

        private string? FormateVehicle(Vehicle vehicle) =>
            vehicle != null ? $"{vehicle.Make} | {vehicle.LicensePlate}" : null;
    }
}
