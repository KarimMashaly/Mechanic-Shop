using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Billing;

namespace MechanicShop.Tests.Common.Billing
{
    public static class InvoiceFactory
    {
        public static Result<Invoice> CreateInvoice(
        Guid? id = null,
        Guid? workOrderId = null,
        List<InvoiceLineItem>? items = null,
        decimal? discount = null,
        decimal? taxAmount = null,
        TimeProvider? timeProvider = null)
        {
            return Invoice.Create(
                id ?? Guid.NewGuid(),
                workOrderId ?? Guid.NewGuid(),
                timeProvider ?? TimeProvider.System,
                discount ?? 0, taxAmount ?? 0,
                items ?? [InvoiceLineItemFactory.CreateInvoiceLineItem().Value]);
        }
    }
}
