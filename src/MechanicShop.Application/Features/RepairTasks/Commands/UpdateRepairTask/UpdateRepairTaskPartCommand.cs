using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public sealed record UpdateRepairTaskPartCommand(
        Guid? PartId,
        string Name,
        int Quantity,
        decimal Cost) 
        : IRequest<Result<Updated>>;
}
