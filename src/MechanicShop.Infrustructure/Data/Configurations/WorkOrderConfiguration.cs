using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Billing;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanicShop.Infrustructure.Data.Configurations
{
    public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            builder.HasKey(wo => wo.Id)
                .IsClustered(false);

            builder.Property(wo => wo.StartAtUtc)
                .IsRequired();

            builder.Property(wo => wo.EndAtUtc)
                .IsRequired();

            builder.Property(wo => wo.Spot)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(wo => wo.State)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(wo => wo.Discout)
                .HasPrecision(18, 2);

            builder.Property(wo => wo.Tax)
                .HasPrecision(18, 2);

            builder.Property(wo => wo.LaborId)
                .IsRequired();

            builder.HasOne(wo => wo.Labor)
                .WithMany()
                .HasForeignKey(e => e.LaborId);

            builder.Property(wo => wo.VehicleId)
                .IsRequired();

            builder.HasOne(wo => wo.Vehicle)
                .WithMany()
                .HasForeignKey(e => e.VehicleId);

            builder.HasOne(wo => wo.Invoice)
                .WithOne(i => i.WorkOrder)
                .HasForeignKey<Invoice>(i => i.WorkOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(wo => wo.TotalPartsCost);

            builder.Ignore(wo => wo.Total);

            builder.Ignore(wo => wo.TotalLaborCost);

            builder.HasMany(wo => wo.RepairTasks)
                .WithMany()
                .UsingEntity(j => j.ToTable("WorkOrderRepairTasks"));

            builder.Navigation(wo => wo.RepairTasks)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(wo => wo.VehicleId);

            builder.HasIndex(wo => wo.LaborId);

            builder.HasIndex(wo => wo.State);

            builder.HasIndex(wo => new {wo.StartAtUtc, wo.EndAtUtc});
        }
    }
}
