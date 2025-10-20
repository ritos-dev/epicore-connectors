using CSharpFunctionalExtensions;

namespace RTS.Service.Connector.Domain.Invoices.Entities
{
    public class InvoiceLine : Entity<int>
    {
        public string Description { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalLinePrice { get; private set; } // Totalprice = Quantity * UnitPrice 
        public decimal VatRate { get; private set; }
        public decimal LineVatAmount { get; private set; } // VatAmount = TotalLinePrice * VatRate

        public InvoiceLine() { }

        public InvoiceLine(int id, string description, int quantity, decimal unitPrice, decimal vatRate)
        {
            Id = id;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalLinePrice = quantity * unitPrice;
            VatRate = vatRate;
            LineVatAmount = TotalLinePrice * vatRate;
        }

        public InvoiceLine(string description, int quantity, decimal unitPrice, decimal vatRate) : this(0, description, quantity, unitPrice, vatRate)
        {
        }

    }
}
