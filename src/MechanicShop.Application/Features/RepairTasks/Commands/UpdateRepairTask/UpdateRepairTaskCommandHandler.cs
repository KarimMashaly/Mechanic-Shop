using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public class UpdateRepairTaskCommandHandler(IAppDbContext context, ILogger<UpdateRepairTaskCommandHandler> logger, HybridCache cache)
        : IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateRepairTaskCommand command, CancellationToken ct)
        {
            var repairTask = await context.RepairTasks
                .Include(rt => rt.Parts)
                .FirstOrDefaultAsync(rt => rt.Id == command.RepairTaskId, ct);

            if(repairTask is null)
            {
                logger.LogWarning("RepairTask {RepairTaskId} not found for update.", command.RepairTaskId);
                return ApplicationErrors.RepairTaskNotFound;
            }

            var validatedParts = new List<Part>();

            foreach(var part in command.Parts)
            {
                var partId = part.PartId ?? Guid.NewGuid();

                var partResult = Part.Create(partId, part.Name, part.Quantity, part.Cost);

                if (partResult.IsError)
                    return partResult.Errors!;

                validatedParts.Add(partResult.Value);
            }

            var udpateRepairTaskResult = repairTask.Update(command.Name, command.LaborCost, command.EstimatedDurationInMins);

            if (udpateRepairTaskResult.IsError)
                return udpateRepairTaskResult.Errors!;

            var upsertParts = repairTask.UpsertParts(validatedParts);

            if(upsertParts.IsError)
                return upsertParts.Errors!;

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("repair-task", ct);

            return Result.Updated;
        }
    }
}
