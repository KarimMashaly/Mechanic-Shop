using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Scheduling.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Scheduling.Queries.GetDailySchedule
{
    public sealed record GetDailyScheduleQuery(
        TimeZoneInfo TimeZone,
        DateOnly ScheduleDate,
        Guid? LaborId = null)
        : ICachedQuery<Result<ScheduleDto>>
    {
        public string CacheKey => throw new NotImplementedException();

        public string[] Tags => throw new NotImplementedException();

        public TimeSpan Expiration => throw new NotImplementedException();
    }
}
