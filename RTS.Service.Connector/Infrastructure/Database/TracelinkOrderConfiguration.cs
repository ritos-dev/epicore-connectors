using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTS.Service.Connector.Domain.Orders.Entities;

namespace RTS.Service.Connector.Infrastructure.Database
{
    public class TracelinkOrderConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.ToTable("TracelinkOrders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber)
                .IsRequired();

            builder.Property(o => o.CompanyId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o=>o.CrmId)
                .IsRequired();
        }
    }
}
