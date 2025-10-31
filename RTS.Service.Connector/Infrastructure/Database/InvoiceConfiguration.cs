using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTS.Service.Connector.Domain.Invoices.Entities;

namespace RTS.Service.Connector.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(i => i.CustomerName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(i => i.Currency)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(i => i.Status)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(i => i.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0);

            builder.Property(i => i.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(i => i.OrderNumber)
                   .HasDatabaseName("IX_Invoices_OrderNumber");
        }
    }
}
