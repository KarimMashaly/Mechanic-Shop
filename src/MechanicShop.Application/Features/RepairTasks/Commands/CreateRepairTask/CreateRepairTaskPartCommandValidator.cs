using FluentValidation;


namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public sealed class CreateRepairTaskPartCommandValidator : AbstractValidator<CreateRepairTaskPartCommand>
    {
        public CreateRepairTaskPartCommandValidator()
        {
            RuleFor(rt => rt.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100);

            RuleFor(rt => rt.Cost)
                .GreaterThan(0).WithMessage("Part cost must be grater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }


}
