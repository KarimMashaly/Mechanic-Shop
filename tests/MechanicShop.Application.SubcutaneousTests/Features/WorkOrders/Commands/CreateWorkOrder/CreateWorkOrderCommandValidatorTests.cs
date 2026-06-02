using MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Tests.Common.WorkOrders;

using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.CreateWorkOrder
{
    public class CreateWorkOrderCommandValidatorTests
    {
        private readonly CreateWorkOrderCommandValidator _validator;

        public CreateWorkOrderCommandValidatorTests()
        {
            _validator = new CreateWorkOrderCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_VehicleId_Is_Empty()
        {
            var command = WorkOrderCommandFactory.CreateCreateWorkOrderCommand(vehicleId: Guid.Empty);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);

            Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.VehicleId));
        }

        [Fact]
        public void Should_Have_Error_When_StartAt_Is_Not_In_Future()
        {
            var command = WorkOrderCommandFactory.CreateCreateWorkOrderCommand(startAt: DateTime.UtcNow.AddSeconds(-1));

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.StartAtUtc));
        }

        [Fact]
        public void Should_Have_Error_When_RepairTaskIds_Is_Empty()
        {
            var command = WorkOrderCommandFactory.CreateCreateWorkOrderCommand(repairTaskIds: []);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.RepairTaskIds));
        }

        [Fact]
        public void Should_Have_Error_When_LaborId_Is_EmptyGuid()
        {
            var command = WorkOrderCommandFactory.CreateCreateWorkOrderCommand(laborId: Guid.Empty);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.LaborId));
        }

        [Fact]
        public void Should_Have_Error_When_Spot_Is_Invalid()
        {
            var command = WorkOrderCommandFactory.CreateCreateWorkOrderCommand(spot: (Spot)999);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.Spot));
        }

        [Fact]
        public void Should_Pass_When_Valid()
        {
            var command = WorkOrderCommandFactory.CreateCreateWorkOrderCommand();

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }
    }
}
