using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState
{
    public sealed class UpdateWorkOrderStateCommandValidator : AbstractValidator<UpdateWorkOrderStateCommand>
    {
        public UpdateWorkOrderStateCommandValidator()
        {
            RuleFor(wo => wo.WorkOrderId)
                .NotEmpty()
                .WithErrorCode("WorkOrderId_Required")
                .WithMessage("WorkOrderId is required");

            RuleFor(wo => wo.newState)
                .IsInEnum()
                .WithErrorCode("WorkOrderStatus_Invalid")
                .WithMessage("Status must be a valid WorkOrderStatus value.");
        }
    }
}
