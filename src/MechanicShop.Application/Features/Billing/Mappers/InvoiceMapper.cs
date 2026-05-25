using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.WorkOrders.Billing;

namespace MechanicShop.Application.Features.Billing.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceDto ToDto(this Invoice entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new InvoiceDto
            {
                InvoiceId = entity.Id,
                WorkOrderId = entity.WorkOrderId,
                Customer = entity.WorkOrder!.Vehicle!.Customer!.ToDto(),
                Vehicle = entity.WorkOrder.Vehicle.ToDto(),
                IssuedAtUtc = entity.IssuedAtUtc,
                Subtotal = entity.SubTotal,
                TaxAmount = entity.TaxAmount,
                DiscountAmount = entity.DiscountAmount,
                Total = entity.Total,
                PaymentStatus = entity.Status.ToString(),
                Items = entity.LineItems.Select(x => x.ToDto()).ToList()
            };
        }

        public static List<InvoiceDto> ToDtos(this IEnumerable<Invoice> entities)
        {
            return [.. entities.Select(e => e.ToDto())];
        }

        public static InvoiceLineItemDto ToDto(this InvoiceLineItem entity)
        {
            return new InvoiceLineItemDto
            {
                InvoiceId = entity.InvoiceId,
                LineNumber = entity.LineNumber,
                Description = entity.Description,
                Quantity = entity.Qunatity,
                UnitPrice = entity.UnitPrice,
                LineTotal = entity.LineTotal
            };
        }

        public static List<InvoiceLineItemDto> ToDtos(this IEnumerable<InvoiceLineItem> entities)
        {
            return [.. entities.Select(e => e.ToDto())];
        }
    }
}
