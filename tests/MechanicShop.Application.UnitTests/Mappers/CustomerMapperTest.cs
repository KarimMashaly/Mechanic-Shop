using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Tests.Common.Customers;

using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers
{
    public class CustomerMapperTest
    {
        [Fact]
        public void ToDto_ShouldMapCorrectly()
        {
            var vehicle = VehicleFactory.CreateVehicle().Value;
            var customer = CustomerFactory.CreateCustomer(vehicles: [vehicle]).Value;

            var dto = customer.ToDto();

            Assert.Equal(customer.Id, dto.CustomerId);
            Assert.Equal(customer.Name, dto.Name);
            Assert.Equal(customer.Email, dto.Email);
            Assert.Equal(customer.Phone, dto.PhoneNumber);

            Assert.Single(dto.Vehicles);

            var vehicleDto = dto.Vehicles.Single();

            Assert.Equal(vehicle.Id, vehicleDto.VehicleId);
            Assert.Equal(vehicle.LicensePlate, vehicleDto.LicensePlate);
            Assert.Equal(vehicle.Model, vehicleDto.Model);
            Assert.Equal(vehicle.Make, vehicleDto.Make);
            Assert.Equal(vehicle.Year, vehicleDto.Year);
        }

        [Fact]
        public void ToDtos_ShouldMapCorrectly()
        {
            //Arrange
            var vehicle = VehicleFactory.CreateVehicle().Value;
            var customer = CustomerFactory.CreateCustomer( vehicles: [vehicle]).Value;

            var customers = new List<Customer> { customer };
            var vehicles = new List<Vehicle> { vehicle };

            //Act
            var customerDtos = customers.ToDtos();

            //Assert
            Assert.Single(customerDtos);

            var customerDto = customerDtos.Single();

            Assert.Equal(customer.Id, customerDto.CustomerId);
            Assert.Equal(customer.Name, customerDto.Name);
            Assert.Equal(customer.Email, customerDto.Email);
            Assert.Equal(customer.Phone, customerDto.PhoneNumber);

            Assert.Single(customerDto.Vehicles);

            var vehicleDto = customerDto.Vehicles.Single();

            Assert.Equal(vehicle.Id, vehicleDto.VehicleId);
            Assert.Equal(vehicle.LicensePlate, vehicleDto.LicensePlate);
            Assert.Equal(vehicle.Model, vehicleDto.Model);
            Assert.Equal(vehicle.Make, vehicleDto.Make);
            Assert.Equal(vehicle.Year, vehicleDto.Year);
        }
    }
}
