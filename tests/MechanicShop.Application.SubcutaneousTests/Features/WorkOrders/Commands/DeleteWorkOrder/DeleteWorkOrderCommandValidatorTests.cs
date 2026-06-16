using MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.DeleteWorkOrder
{
    public class DeleteWorkOrderCommandValidatorTests
    {
        private readonly DeleteWorkOrderCommandValidator _validator;

        public DeleteWorkOrderCommandValidatorTests()
        {
            _validator = new DeleteWorkOrderCommandValidator();
        }
    }
}
