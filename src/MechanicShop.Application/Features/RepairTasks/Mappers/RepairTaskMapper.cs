using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Application.Features.RepairTasks.Mappers
{
    public static class RepairTaskMapper
    {
        public static RepairTaskDto ToDto(this RepairTask entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new RepairTaskDto
            {
                Name = entity.Name!,
                LaborCost = entity.LaborCost,
                TotalCost = entity.TotalCost,
                RepairTaskId = entity.Id,
                EstimatedDurationInMins = entity.EstimatedDurationInMins,
                Parts = entity.Parts.ToList().ConvertAll(ToDto)
            };
        }
        public static List<RepairTaskDto> ToDtos(this IEnumerable<RepairTask> entities)
        {
            return [.. entities.Select(rt => rt.ToDto()).ToList()];
        }

        public static PartDto ToDto(this Part entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new PartDto
            {
                Name = entity.Name!,
                Quantity = entity.Quantity,
                Cost = entity.Cost,
                PartId = entity.Id,
            };
        }

        public static List<PartDto> ToDtos(this IEnumerable<Part> entities)
        {
            return [.. entities.Select(p => p.ToDto()).ToList()];
        }
    }
}
