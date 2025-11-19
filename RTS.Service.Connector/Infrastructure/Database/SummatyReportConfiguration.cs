using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTS.Service.Connector.Domain.SummaryInvoiceReport.Entities;

namespace RTS.Service.Connector.Infrastructure.Database
{
    public class SummatyReportConfiguration : IEntityTypeConfiguration<SummaryReport>
    {
        public void Configure(EntityTypeBuilder<SummaryReport> builder)
        {
            builder.ToTable("SummaryInvoiceReports");

            builder.HasKey(sir => sir.Id);

            builder.Property(sir => sir.CrmId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(sir => sir.CustomerName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(sir => sir.Currency)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(sir => sir.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.HasMany(sir => sir.Invoices)
                .WithOne(sir => sir.SummaryReport)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
