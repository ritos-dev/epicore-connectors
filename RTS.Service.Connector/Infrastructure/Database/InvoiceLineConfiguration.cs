using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RTS.Service.Connector.Domain.Invoices.Entities;

namespace RTS.Service.Connector.Infrastructure.Persistence.Configurations
{
    public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> builder)
        {
            builder.ToTable("InvoiceLines");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.ProductNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(l => l.Description)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(l => l.Quantity)
                   .HasColumnType("decimal(18,2)");

            builder.Property(l => l.UnitPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(l => l.DiscountPercent)
                   .HasColumnType("decimal(5,2)");

            builder.Property(l => l.VatRate)
                   .HasColumnType("decimal(5,2)");

            builder.Property(l => l.LineTotal)
                   .HasColumnType("decimal(18,2)");

            // Relationship
            builder.HasOne(l => l.Invoice)
                   .WithMany(i => i.Lines)
                   .HasForeignKey(l => l.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
