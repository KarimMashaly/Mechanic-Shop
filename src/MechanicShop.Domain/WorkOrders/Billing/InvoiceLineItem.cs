using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing
{
    public sealed class InvoiceLineItem
    {
        public int LineNumber { get; }
        public string? Description { get; }
        public int Qunatity { get; }
        public decimal UnitPrice { get; }
        public decimal LineTotal => Qunatity * UnitPrice;
        public Guid InvoiceId { get; }

        private InvoiceLineItem() { }

        private InvoiceLineItem(Guid invoiceId, int lineItem, string description, int quantity, decimal unitPrice)
        {
            InvoiceId = invoiceId;
            LineNumber = lineItem;
            Description = description;
            Qunatity = quantity;
            UnitPrice = unitPrice;
        }

        public static Result<InvoiceLineItem> Create(Guid invoiceId, int lineNumber, string description, int quantity, decimal unitPrice)
        {
            if (invoiceId == Guid.Empty)
                return InvoiceLineItemErrors.InvoiceIdRequired;

            if (lineNumber <= 0)
                return InvoiceLineItemErrors.LineNumberInvalid;

            if(string.IsNullOrWhiteSpace(description))
                return InvoiceLineItemErrors.DescriptionRequired;

            if(quantity <= 0)
                return InvoiceLineItemErrors.QuantityInvalid;

            if(unitPrice <= 0)
                return InvoiceLineItemErrors.UnitPriceInvalid;

            return new InvoiceLineItem(invoiceId, lineNumber, description.Trim(), quantity, unitPrice);
        }

    }
        
}
