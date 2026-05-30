using MechanicShop.Domain.WorkOrders.Billing;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanciShop.Infrustructure.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id)
                .IsClustered(false);

            builder.Property(i => i.IssuedAtUtc)
                .IsRequired();

            builder.Property(i => i.DiscountAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(i => i.TaxAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Ignore(i => i.Total);

            builder.Ignore(i => i.SubTotal);

            builder.Property(i => i.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Navigation(i => i.LineItems)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(i => i.LineItems, items =>
                {
                    items.ToTable("InvoiceLineItems");

                    items.WithOwner()
                    .HasForeignKey(i => i.InvoiceId);

                    items.HasKey(i => new { i.InvoiceId, i.LineNumber });

                    items.Property(i => i.LineNumber)
                    .ValueGeneratedNever();

                    items.Property(i => i.UnitPrice)
                    .HasPrecision(18, 2)
                    .IsRequired();

                    items.Property(i => i.Qunatity)
                    .IsRequired();

                    items.Property(i => i.Description)
                    .IsRequired()
                    .HasMaxLength(200);
                });
        }
    }
}
