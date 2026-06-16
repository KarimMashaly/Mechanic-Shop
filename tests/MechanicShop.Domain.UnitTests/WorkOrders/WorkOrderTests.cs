using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Tests.Common.RepairTasks;
using MechanicShop.Tests.Common.WorkOrders;
using Xunit;

namespace MechanicShop.Domain.UnitTests.WorkOrders
{
    public class WorkOrderTests
    {
        [Fact]
        public void Create_ShouldReturnError_WhenIdIsEmpty()
        {
            var wo = WorkOrderFactory.CreateWorkOrder(id: Guid.Empty);

            Assert.False(wo.IsSuccess);

            Assert.Equal(WorkOrderErrors.WorkOrderIdRequired.Code, wo.TopError.Code);
        }

        [Fact]
        public void Create_ShouldReturnError_WhenVehicleIdIsEmpty()
        {
            var wo = WorkOrderFactory.CreateWorkOrder(vehicleId: Guid.Empty);

            Assert.False(wo.IsSuccess);

            Assert.Equal(WorkOrderErrors.VehicleIdRequired.Code, wo.TopError.Code);
        }

        [Fact]
        public void Create_ShouldReturnError_WhenNoRepairTasks()
        {
            var wo = WorkOrderFactory.CreateWorkOrder(repairTasks: []);

            Assert.False(wo.IsSuccess);

            Assert.Equal(WorkOrderErrors.RepairTasksRequired.Code, wo.TopError.Code);
        }

        [Fact]
        public void Create_ShouldReturnError_WhenLaborIdIsEmpty()
        {
            var wo = WorkOrderFactory.CreateWorkOrder(laborId: Guid.Empty);

            Assert.False(wo.IsSuccess);

            Assert.Equal(WorkOrderErrors.LaborIdRequired.Code, wo.TopError.Code);
        }

        [Fact]
        public void Create_ShouldReturnError_WhenTimingInvalid()
        {
            var wo = WorkOrderFactory.CreateWorkOrder(
                               startAt: DateTimeOffset.UtcNow.AddHours(1),
                               endAt: DateTimeOffset.UtcNow);

            Assert.False(wo.IsSuccess);

            Assert.Equal(WorkOrderErrors.InvalidTiming.Code, wo.TopError.Code);
        }

        [Fact]
        public void Create_ShouldReturnError_WhenSpotInvalid()
        {
            const Spot invalidSpot = (Spot)999;

            var wo = WorkOrderFactory.CreateWorkOrder(spot: invalidSpot);

            Assert.False(wo.IsSuccess);

            Assert.Equal(WorkOrderErrors.SpotInvalid.Code, wo.TopError.Code);
        }

        [Fact]
        public void AddRepairTask_ShouldReturnError_WhenNotEditable()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            wo.UpdateState(WorkOrderState.InProgress);
            wo.UpdateState(WorkOrderState.Completed);

            var result = wo.AddRepairTask(RepairTaskFactory.CreateRepairTask().Value);

            Assert.False(result.IsSuccess);
            Assert.True(result.Errors!.Count > 0);
        }

        [Fact]
        public void UpdateLabor_ShouldReturnError_WhenLaborIdEmpty()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            var result = wo.UpdateLabor(Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal("WorkOrderErrors.LaborIdRequired", result.TopError.Code);
        }

        [Fact]
        public void UpdateSpot_ShouldReturnError_WhenSpotInvalid()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            const Spot invalidSpot = (Spot)999;
            var result = wo.UpdateSpot(invalidSpot);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.SpotInvalid.Code, result.TopError.Code);
        }

        [Fact]
        public void UpdateTiming_ShouldReturnError_WhenTimingInvalid()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            var result = wo.UpdateTiming(DateTimeOffset.UtcNow.AddHours(2), DateTimeOffset.UtcNow);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidTiming.Code, result.TopError.Code);
        }

        [Fact]
        public void UpdateState_ShouldReturnError_WhenTransitionInvalid()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            var result = wo.UpdateState(WorkOrderState.Completed);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.InvalidStateTransition(WorkOrderState.Scheduled, WorkOrderState.Completed).Code, result.TopError.Code);
        }

        [Fact]
        public void UpdateLabor_ShouldReturnSuccess_AndSetNewLaborId()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            var newLabor = Guid.NewGuid();
            var result = wo.UpdateLabor(newLabor);

            Assert.True(result.IsSuccess);
            Assert.Equal(newLabor, wo.LaborId);
        }

        [Fact]
        public void UpdateSpot_ShouldReturnSuccess_AndSetNewSpot()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            var result = wo.UpdateSpot(Spot.B);

            Assert.True(result.IsSuccess);
            Assert.Equal(Spot.B, wo.Spot);
        }

        [Fact]
        public void UpdateTiming_ShouldReturnSuccess_AndSetNewTiming()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            var newStart = wo.StartAtUtc.AddHours(2);
            var newEnd = newStart.AddHours(1);
            var result = wo.UpdateTiming(newStart, newEnd);

            Assert.True(result.IsSuccess);
            Assert.Equal(newStart, wo.StartAtUtc);
            Assert.Equal(newEnd, wo.EndAtUtc);
        }

        [Fact]
        public void UpdateState_ShouldReturnSuccess_AndSetStateToInProgress()
        {
            var wo = WorkOrderFactory.CreateWorkOrder().Value;

            var result = wo.UpdateState(WorkOrderState.InProgress);

            Assert.True(result.IsSuccess);
            Assert.Equal(WorkOrderState.InProgress, wo.State);
        }
    }
}
