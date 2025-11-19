using Microsoft.EntityFrameworkCore;

using RTS.Service.Connector.Domain.Orders.Entities;
using RTS.Service.Connector.Domain.Invoices.Entities;
using RTS.Service.Connector.Domain.SummaryInvoiceReport.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Economic
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<SummaryReport> SummaryInvoiceReports { get; set; }

    // Tracelink
    public DbSet<Orders> TracelinkOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
