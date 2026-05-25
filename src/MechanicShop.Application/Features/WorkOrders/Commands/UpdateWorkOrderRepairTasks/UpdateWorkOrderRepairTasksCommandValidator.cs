using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks
{
    public sealed class UpdateWorkOrderRepairTasksCommandValidator : AbstractValidator<UpdateWorkOrderRepairTasksCommand>
    {
        public UpdateWorkOrderRepairTasksCommandValidator()
        {
            RuleFor(wo => wo.workOrderId)
                .NotEmpty()
                .WithErrorCode("WorkOrderId_Required")
                .WithMessage("WorkOrderId is required");

            RuleFor(x => x.RepairTaskIds)
              .NotEmpty()
              .WithErrorCode("RepairTasks_Required")
              .WithMessage("At least one repair task must be provided.");
        }
    }
}
