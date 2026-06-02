using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Tests.Common.RepairTasks;

using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers
{
    public class RepairTaskMapperTest
    {
        [Fact]
        public void ToDto_ShouldMapCorrectly()
        {
            //Arrange
            var part = PartFactory.CreatePart(cost: 100, quantity: 2).Value;
            var repairTask = RepairTaskFactory.CreateRepairTask(parts: [part]).Value;

            //Act
            var dto = repairTask.ToDto();

            //Assert
            Assert.Equal(repairTask.Id, dto.RepairTaskId);
            Assert.Equal(repairTask.Name, dto.Name);
            Assert.Equal(repairTask.LaborCost, dto.LaborCost);
            Assert.Equal(repairTask.TotalCost, dto.TotalCost);
            Assert.Equal(repairTask.EstimatedDurationInMins, dto.EstimatedDurationInMins);

            Assert.Single(dto.Parts);

            var partDto = dto.Parts.Single();

            Assert.Equal(part.Id, partDto.PartId);
            Assert.Equal(part.Name, partDto.Name);
            Assert.Equal(part.Cost, partDto.Cost);
            Assert.Equal(part.Quantity, partDto.Quantity);
        }

        [Fact]
        public void ToDtos_ShouldMapCorrectly()
        {
            //Arrange
            var part = PartFactory.CreatePart(cost: 100, quantity: 2).Value;
            var repairTask = RepairTaskFactory.CreateRepairTask(parts: [part]).Value;

            var repairTasks = new List<RepairTask> { repairTask };

            //Act
            var dtos = repairTasks.ToDtos();

            //Assert
            Assert.Single(dtos);

            var dto = dtos.Single();

            Assert.Equal(repairTask.Id, dto.RepairTaskId);
            Assert.Equal(repairTask.Name, dto.Name);
            Assert.Equal(repairTask.LaborCost, dto.LaborCost);
            Assert.Equal(repairTask.TotalCost, dto.TotalCost);
            Assert.Equal(repairTask.EstimatedDurationInMins, dto.EstimatedDurationInMins);

            Assert.Single(dto.Parts);

            var partDto = dto.Parts.Single();

            Assert.Equal(part.Id, partDto.PartId);
            Assert.Equal(part.Name, partDto.Name);
            Assert.Equal(part.Cost, partDto.Cost);
            Assert.Equal(part.Quantity, partDto.Quantity);
        }
    }
}
