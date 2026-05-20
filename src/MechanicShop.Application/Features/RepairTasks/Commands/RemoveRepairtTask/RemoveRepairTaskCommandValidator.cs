using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairtTask
{
    public sealed class RemoveRepairTaskCommandValidator : AbstractValidator<RemoveRepairTaskCommand>
    {
        public RemoveRepairTaskCommandValidator()
        {
            RuleFor(rt => rt.RepairTaskId)
                .NotEmpty().WithMessage("Repair task id is required");
        }
    }
}
