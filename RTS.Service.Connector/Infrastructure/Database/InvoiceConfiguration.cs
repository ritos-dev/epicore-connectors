using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTS.Service.Connector.Domain.Invoices.Entities;

namespace RTS.Service.Connector.Infrastructure.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.CrmId)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(i => i.TLOrderNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(i => i.CustomerName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(i => i.Currency)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(i => i.InvoiceCreateDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.InvoiceAmount)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0);

            builder.Property(i => i.InvoiceNumber);

            builder.Property(i => i.Status)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasMany(i => i.Lines) 
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
