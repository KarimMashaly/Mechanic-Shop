using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public sealed class UpdateRepairTaskPartCommandValidator : AbstractValidator<UpdateRepairTaskPartCommand>
    {
        public UpdateRepairTaskPartCommandValidator()
        {
            RuleFor(rt => rt.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100);

            RuleFor(rt => rt.Cost)
                .InclusiveBetween(1, 10_000).WithMessage("Cost must be between 1 and 10.000.");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 10).WithMessage("Quantity must be between 1 and 10.");
        }
    }
}
