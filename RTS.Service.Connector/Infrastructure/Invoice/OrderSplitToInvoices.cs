using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Interfaces;

namespace RTS.Service.Connector.Infrastructure.InvoiceSplit
{
    public class InvoicePart
    {
        public string Description { get; set; } = string.Empty;
        public decimal NetPrice { get; set; }
        public decimal Percentage { get; set; }
        public string? ProductNumber { get; set; }
    }
    public class OrderSplitToInvoices : IOrderSplitToInvoices
    {
        private readonly List<InvoicePart> _templateParts;
        public OrderSplitToInvoices(List<InvoicePart>? templateParts = null)
        {
            _templateParts = templateParts ?? new List<InvoicePart>
            {
                new InvoicePart { Description = "Depositum (10%)", Percentage = 0.10m, ProductNumber = "4" },
                new InvoicePart { Description = "1. Rate (50%)",   Percentage = 0.50m, ProductNumber = "5" },
                new InvoicePart { Description = "2. Rate (40%)",   Percentage = 0.40m, ProductNumber = "6" }
            };
        }

        public List<InvoicePart> Split(decimal totalAmount, CustomerType customerType)
        {
            if (customerType != CustomerType.Private)
            {
                // B2B NOT implemented yet
                return new List<InvoicePart>();
            }

            return _templateParts.Select(p => new InvoicePart
            {
                Description = p.Description,
                Percentage = p.Percentage,
                NetPrice = Math.Round(totalAmount * p.Percentage, 2, MidpointRounding.AwayFromZero),
                ProductNumber = p.ProductNumber
            }).ToList();
        }
    }
}
