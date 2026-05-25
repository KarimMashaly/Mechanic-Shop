using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder
{
    public sealed record CreateWorkOrderCommand(
        Guid? LaborId,
        Guid VehicleId,
        DateTimeOffset StartAtUtc,
        Spot Spot,
        List<Guid> RepairTaskIds) 
        : IRequest<Result<WorkOrderDto>>;
}
