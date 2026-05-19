using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;


namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public sealed class CreateRepairTaskCommandHandler(
        IAppDbContext context,
        ILogger<CreateRepairTaskCommandHandler> logger,
        HybridCache cache) : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
    {
        public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand command, CancellationToken ct)
        {
            var nameExists = await context.RepairTasks
                .AnyAsync(rt => EF.Functions.Like(rt.Name, command.Name), ct);

            if(nameExists)
            {
                logger.LogWarning("Duplicate part name '{PartName}'.", command.Name);

                return RepairTaskErrors.DuplicateName;
            }

            List<Part> parts = [];

            foreach (var part in command.Parts)
            {
                var createPartResult = Part.Create(Guid.NewGuid(), part.Name, part.Quantity, part.Cost);

                if (createPartResult.IsError)
                    return createPartResult.Errors!;

                parts.Add(createPartResult.Value);
            }

            var createRepairTaskResult = RepairTask.Create(Guid.NewGuid(), command.Name, command.LaborCost, command.EstimatedDurationInMins, parts);

            if(createRepairTaskResult.IsError)
                return createRepairTaskResult.Errors!;

            var repairTask = createRepairTaskResult.Value;

            context.RepairTasks.Add(repairTask);

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("repair-task", ct);

            return repairTask.ToDto();
        }
    }
}
