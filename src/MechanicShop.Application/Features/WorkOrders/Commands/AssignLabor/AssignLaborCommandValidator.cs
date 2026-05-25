using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor
{
    public sealed class AssignLaborCommandValidator: AbstractValidator<AssignLaborCommand>
    {
        public AssignLaborCommandValidator()
        {
            RuleFor(l => l.WorkOrderId)
                .NotEmpty()
                .WithErrorCode("WorkOrderId_Required")
                .WithMessage("WorkOrderId is required");

            RuleFor(l => l.LaborId)
                .NotEmpty()
                .WithErrorCode("LaborId_Required")
                .WithMessage("LaborId is required");
        }
    }
}
