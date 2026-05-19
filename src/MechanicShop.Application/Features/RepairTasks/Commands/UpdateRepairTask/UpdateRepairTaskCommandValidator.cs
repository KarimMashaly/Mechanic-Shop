using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public sealed class UpdateRepairTaskCommandValidator : AbstractValidator<UpdateRepairTaskCommand>
    {
        public UpdateRepairTaskCommandValidator()
        {
            RuleFor(rt =>  rt.RepairTaskId)
                .NotEmpty().WithMessage("Repair task id is required");

            RuleFor(rt => rt.Name)
               .NotEmpty().WithMessage("Name is required")
               .MaximumLength(100);

            RuleFor(rt => rt.LaborCost)
                .InclusiveBetween(1, 10_000)
            .WithMessage("Labor cost must be between 1 and 10,000.");

            RuleFor(rt => rt.EstimatedDurationInMins)
                .IsInEnum()
                .WithMessage("Invalid duration selected");

            RuleFor(rt => rt.Parts)
                .NotNull()
                .Must(p => p.Count > 0).WithMessage("At least one part is required");

            RuleForEach(rt => rt.Parts).SetValidator(new UpdateRepairTaskPartCommandValidator());
        }
    }
}
