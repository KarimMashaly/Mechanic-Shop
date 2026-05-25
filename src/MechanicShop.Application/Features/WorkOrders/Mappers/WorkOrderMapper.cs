using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.WorkOrders;

namespace MechanicShop.Application.Features.WorkOrders.Mappers
{
    public static class WorkOrderMapper
    {
        public static WorkOrderDto ToDto(this WorkOrder entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new WorkOrderDto
            {
                WorkOrderId = entity.Id,
                StartAtUtc = entity.StartAtUtc,
                EndAtUtc = entity.EndAtUtc,
                CreatedAt = entity.CreatedAtUtc,
                InvoiceId = entity.Invoice?.Id,
                Spot = entity.Spot,
                State = entity.State,
                TotalDurationInMins = entity.RepairTasks.Sum(r => (int)r.EstimatedDurationInMins),
                TotalLaborCost = entity.RepairTasks.Sum(r => r.LaborCost),
                TotalPartCost = entity.RepairTasks.SelectMany(r => r.Parts).Sum(p => p.Quantity * p.Cost),
                TotalCost = entity.RepairTasks.Sum(rt => rt.TotalCost),
                Vehicle = entity.Vehicle is null ? null : entity.Vehicle.ToDto(),
                Labor = entity.Labor is null ? null : new LaborDto
                {
                    LaborId = entity.LaborId,
                    Name = $"{entity.Labor.FirstName} {entity.Labor.LastName}"
                },
                RepairTasks = entity.RepairTasks.ToDtos()

            };
        }

        public static List<WorkOrderDto> ToDtos (this IEnumerable<WorkOrder> entities)
        {
            return [.. entities.Select(e => e.ToDto())]; 
        }
    }
}
