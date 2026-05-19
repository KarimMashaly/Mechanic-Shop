using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing
{
    public sealed class Invoice : AuditableEntity
    {
        public DateTimeOffset IssuedAtUtc { get; }
        public decimal DiscountAmount { get; private set; }
        public decimal TaxAmount { get; private set; }
        public InvoiceStatus Status { get; private set; }

        public decimal SubTotal => _lineItems.Sum(it => it.LineTotal);
        public decimal Total => SubTotal - DiscountAmount + TaxAmount;

        public DateTimeOffset PaidAt { get; private set; }

        public Guid WorkOrderId { get; set; }
        public WorkOrder? WorkOrder { get; set; }

        private readonly List<InvoiceLineItem> _lineItems = [];
        public IEnumerable<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

        private Invoice() { }

        private Invoice(Guid id, Guid workOrderId, DateTimeOffset issuedAt, decimal discountAmount,
            decimal taxAmount, List<InvoiceLineItem> lineItems)
            : base(id)
        {
            IssuedAtUtc = issuedAt;
            DiscountAmount = discountAmount;
            TaxAmount = taxAmount;
            _lineItems = lineItems;
            WorkOrderId = workOrderId;
        }

        public static Result<Invoice> Create(Guid id, Guid workOrderId, TimeProvider datetime, decimal discountAmount,
            decimal taxAmount, List<InvoiceLineItem> lineItems)
        {
            if (workOrderId == Guid.Empty)
                return InvoiceErrors.WorkOrderIdInvalid;

            if (lineItems is null || lineItems.Count <= 0)
                return InvoiceErrors.LineItemsEmpty;

            return new Invoice(id, workOrderId, datetime.GetUtcNow(), discountAmount, taxAmount, lineItems);
        }

        public Result<Updated> ApplyDiscount(decimal discountAmount)
        {
            if (Status != InvoiceStatus.Unpaid)
                return InvoiceErrors.InvoiceLocked;

            if (DiscountAmount < 0)
                return InvoiceErrors.DiscountNegative;

            if (discountAmount > SubTotal)
                return InvoiceErrors.DiscountExceedsSubtotal;

            DiscountAmount = discountAmount;

            return Result.Updated;
        }

        public Result<Updated> MarkAsPaid(TimeProvider timeProvider)
        {
            if (Status != InvoiceStatus.Unpaid)
                return InvoiceErrors.InvoiceLocked;

            Status = InvoiceStatus.Paid;
            PaidAt = timeProvider.GetUtcNow();

            return Result.Updated;
        }
    }
        
}
