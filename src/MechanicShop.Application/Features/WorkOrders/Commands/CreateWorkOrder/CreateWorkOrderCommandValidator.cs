using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder
{
    public sealed class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
    {
        public CreateWorkOrderCommandValidator()
        {
            RuleFor(wo => wo.VehicleId)
                .NotEmpty()
                .WithMessage("Vehicle id is required");

            RuleFor(request => request.LaborId)
             .Must(laborId => laborId is null || laborId != Guid.Empty)
             .WithMessage("If provided, LaborId must not be empty.");

            RuleFor(wo => wo.StartAtUtc)
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage("StartAt must be in the future.");

            RuleFor(wo => wo.RepairTaskIds)
                .NotEmpty()
                .WithMessage("At least one repair task must be selected");

            RuleFor(wo => wo.Spot)
                .IsInEnum()
                .WithErrorCode("Spot_Invalid")
                .WithMessage("Spot must be a valid Spot value. [A, B, C, D]");
        }
    }
}
