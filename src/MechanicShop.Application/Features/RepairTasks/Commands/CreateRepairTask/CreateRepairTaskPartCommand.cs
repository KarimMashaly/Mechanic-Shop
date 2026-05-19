using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;


namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public sealed record CreateRepairTaskPartCommand(
        string Name,
        int Quantity,
        decimal Cost
        ) : IRequest<Result<PartDto>>;
}
