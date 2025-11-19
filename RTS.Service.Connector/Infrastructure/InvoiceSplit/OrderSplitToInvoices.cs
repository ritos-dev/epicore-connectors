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
        public List<InvoicePart> Split(decimal totalAmount, CustomerType customerType)
        {
            if (customerType != CustomerType.Private)
            {
                // B2B NOT implemented yet return nothing
                return new List<InvoicePart>();
            }

            return new List<InvoicePart>
            {
                new InvoicePart
                {
                    Description = "Depositum (10%)",
                    Percentage = 0.10m,
                    NetPrice = Math.Round(totalAmount * 0.10m, 2, MidpointRounding.AwayFromZero),
                    ProductNumber = "4", // Will be changed to equivalent "Varenummer" in RTS economic 
                },
                new InvoicePart
                {
                    Description = "1. Rate (50%)",
                    Percentage = 0.50m,
                    NetPrice = Math.Round(totalAmount * 0.50m, 2, MidpointRounding.AwayFromZero),
                    ProductNumber = "5", // Will be changed to equivalent "Varenummer" in RTS economic 
                },
                new InvoicePart
                {
                    Description = "2. Rate (40%)",
                    Percentage = 0.40m,
                    NetPrice = Math.Round(totalAmount * 0.40m, 2, MidpointRounding.AwayFromZero),
                    ProductNumber = "6", // Will be changed to equivalent "Varenummer" in RTS economic 
                },
            };
        }
    }
}
