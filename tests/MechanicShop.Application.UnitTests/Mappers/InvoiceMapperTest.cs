using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Tests.Common.Billing;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.WorkOrders;

using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers
{
    public class InvoiceMapperTest
    {
        [Fact]
        public void ToDto_ShouldMapCorrectly()
        {
            // Arrange
            var vehicle = VehicleFactory.CreateVehicle().Value;
            var customer = CustomerFactory.CreateCustomer(vehicles: [vehicle]).Value;
            vehicle.Customer = customer;

            var workOrder = WorkOrderFactory.CreateWorkOrder(vehicleId: vehicle.Id).Value;
            workOrder.Vehicle = vehicle;

            var invoieLineItem = InvoiceLineItemFactory.CreateInvoiceLineItem().Value;
            var invoice = InvoiceFactory.CreateInvoice(items: [invoieLineItem], workOrderId: workOrder.Id).Value;
            invoice.WorkOrder = workOrder;
            var status = invoice.Status.ToString();

            // Act
            var dto = invoice.ToDto();

            // Assert
            Assert.Equal(invoice.Id, dto.InvoiceId);
            Assert.Equal(invoice.WorkOrderId, dto.WorkOrderId);
            Assert.Equal(invoice.IssuedAtUtc, dto.IssuedAtUtc);
            Assert.Equal(invoice.SubTotal, dto.Subtotal);
            Assert.Equal(invoice.TaxAmount, dto.TaxAmount);
            Assert.Equal(invoice.DiscountAmount, dto.DiscountAmount);
            Assert.Equal(invoice.Total, dto.Total);
            Assert.Equal(status, dto.PaymentStatus);

            Assert.NotNull(dto.Customer);

            var customerDto = dto.Customer;
            Assert.Equal(customer.Id, customerDto.CustomerId);
            Assert.Equal(customer.Name, customerDto.Name);
            Assert.Equal(customer.Email, customerDto.Email);
            Assert.Equal(customer.Phone, customerDto.PhoneNumber);

            Assert.NotNull(dto.Vehicle);

            var vehicleDto = dto.Vehicle;
            Assert.Equal(vehicle.Id, vehicleDto.VehicleId);
            Assert.Equal(vehicle.LicensePlate, vehicleDto.LicensePlate);
            Assert.Equal(vehicle.Model, vehicleDto.Model);
            Assert.Equal(vehicle.Make, vehicleDto.Make);
            Assert.Equal(vehicle.Year, vehicleDto.Year);

            Assert.Single(dto.Items);

            var itemDto = dto.Items.Single();

            Assert.Equal(invoieLineItem.InvoiceId, itemDto.InvoiceId);
            Assert.Equal(invoieLineItem.LineNumber, itemDto.LineNumber);
            Assert.Equal(invoieLineItem.Description, itemDto.Description);
            Assert.Equal(invoieLineItem.Quantity, itemDto.Quantity);
            Assert.Equal(invoieLineItem.UnitPrice, itemDto.UnitPrice);
            Assert.Equal(invoieLineItem.LineTotal, itemDto.LineTotal);
        }

        [Fact]
        public void ToDtos_ShouldMapListCorrectly()
        {
            // Arrange
            var vehicle = VehicleFactory.CreateVehicle().Value;
            var customer = CustomerFactory.CreateCustomer(vehicles: [vehicle]).Value;
            vehicle.Customer = customer;

            var workOrder = WorkOrderFactory.CreateWorkOrder(vehicleId: vehicle.Id).Value;
            workOrder.Vehicle = vehicle;

            var invoieLineItem = InvoiceLineItemFactory.CreateInvoiceLineItem().Value;
            var invoice = InvoiceFactory.CreateInvoice(items: [invoieLineItem], workOrderId: workOrder.Id).Value;
            invoice.WorkOrder = workOrder;
            var status = invoice.Status.ToString();

            var invoices = new List<Invoice> { invoice };
            // Act
            var dtos = invoices.ToDtos();

            // Assert
            Assert.Single(dtos);

            var dto = dtos.Single();

            Assert.Equal(invoice.Id, dto.InvoiceId);
            Assert.Equal(invoice.WorkOrderId, dto.WorkOrderId);
            Assert.Equal(invoice.IssuedAtUtc, dto.IssuedAtUtc);
            Assert.Equal(invoice.SubTotal, dto.Subtotal);
            Assert.Equal(invoice.TaxAmount, dto.TaxAmount);
            Assert.Equal(invoice.DiscountAmount, dto.DiscountAmount);
            Assert.Equal(invoice.Total, dto.Total);
            Assert.Equal(status, dto.PaymentStatus);

            Assert.NotNull(dto.Customer);

            var customerDto = dto.Customer;
            Assert.Equal(customer.Id, customerDto.CustomerId);
            Assert.Equal(customer.Name, customerDto.Name);
            Assert.Equal(customer.Email, customerDto.Email);
            Assert.Equal(customer.Phone, customerDto.PhoneNumber);

            Assert.NotNull(dto.Vehicle);

            var vehicleDto = dto.Vehicle;
            Assert.Equal(vehicle.Id, vehicleDto.VehicleId);
            Assert.Equal(vehicle.LicensePlate, vehicleDto.LicensePlate);
            Assert.Equal(vehicle.Model, vehicleDto.Model);
            Assert.Equal(vehicle.Make, vehicleDto.Make);
            Assert.Equal(vehicle.Year, vehicleDto.Year);

            Assert.Single(dto.Items);

            var itemDto = dto.Items.Single();

            Assert.Equal(invoieLineItem.InvoiceId, itemDto.InvoiceId);
            Assert.Equal(invoieLineItem.LineNumber, itemDto.LineNumber);
            Assert.Equal(invoieLineItem.Description, itemDto.Description);
            Assert.Equal(invoieLineItem.Quantity, itemDto.Quantity);
            Assert.Equal(invoieLineItem.UnitPrice, itemDto.UnitPrice);
            Assert.Equal(invoieLineItem.LineTotal, itemDto.LineTotal);
        }
    }
}
