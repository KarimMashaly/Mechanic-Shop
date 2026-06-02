using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Domain.Employees;
using MechanicShop.Tests.Common.Employees;

using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers
{
    public class LaborMapperTest
    {
        [Fact]
        public void ToDto_ShouldMapCorrectly()
        {
            //Arrange
            var labor = EmployeeFactory.CreateLabor().Value;

            //Act
            var dto = labor.ToDto();

            //Assert
            Assert.Equal(labor.Id, dto.LaborId);
            Assert.Equal(labor.FullName, dto.Name);
        }

        [Fact]
        public void ToDtos_ShouldMapListCorrectly()
        {
            //Arrange
            var labor = EmployeeFactory.CreateLabor().Value;
            var labors = new List<Employee> { labor };

            //Act
            var dtos = labors.ToDtos();

            //Assert
            Assert.Single(dtos);

            var dto = dtos.Single();

            Assert.Equal(labor.Id, dto.LaborId);
            Assert.Equal(labor.FullName, dto.Name);
        }
    }
}
