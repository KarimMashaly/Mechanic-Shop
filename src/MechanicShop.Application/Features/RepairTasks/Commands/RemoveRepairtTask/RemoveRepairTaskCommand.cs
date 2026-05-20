using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairtTask
{
    public sealed record RemoveRepairTaskCommand(Guid RepairTaskId) : IRequest<Result<Deleted>>;
}
