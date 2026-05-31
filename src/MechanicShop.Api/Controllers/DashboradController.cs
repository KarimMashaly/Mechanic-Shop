using Asp.Versioning;

using MechanicShop.Application.Features.Dashboard.Dtos;
using MechanicShop.Application.Features.Dashboard.Queries.GetWorkOrderStates;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers
{
    [Route("api/v{version:apiVersion}/dashboard")]
    [ApiVersion("1.0")]
    [Authorize]
    public sealed class DashboradController(ISender sender) : ApiController
    {
        [HttpGet("stats")]
        [ProducesResponseType(typeof(TodayWorkOrderStatsDto), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>GetTodayStats(DateOnly? date, CancellationToken ct)
        {
            var statsDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var result = await sender.Send(new GetWorkOrderStatsQuery(statsDate), ct);

            return result.Match(
                response => Ok(response),
                Problem);
        }
    }
}
