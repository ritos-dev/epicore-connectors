using Ardalis.GuardClauses;
using RTS.SharedKernel.DDD;
using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Domain.Invoices.Entities;

namespace RTS.Service.Connector.Domain.Invoices
{
    public class Invoice : AggregateRoot
    {
        public string ProjectNumber { get; private set; } = string.Empty;
        public Customer Customer { get; private set; } = null!;
        public DateTime? InvoiceCreationDate { get; private set; }
        public bool PaymentTerms { get; private set; }
        public Currency Currency { get; private set; }
        public VatZone VatZone { get; private set; }
        public decimal NetAmount { get; private set; } // Total before VAT
        public decimal VatAmount { get; private set; } // VAT amount based of VatRate 
        public decimal GrossAmount { get; private set; } // Total after VAT 
        public string Notes { get; private set; } = string.Empty;

        private readonly List<InvoiceLine> _invoiceLines = new();
        public IReadOnlyList<InvoiceLine> InvoiceLines => _invoiceLines.AsReadOnly();

        public Invoice() { }
        public Invoice(
            int id,
            string projectNumber,
            Customer customer,
            DateTime? invoiceCreationDate,
            bool paymentTerms,
            Currency currency,
            VatZone vatZone,
            decimal netAmount,
            decimal vatAmount,
            decimal grossAmount,
            string notes)
        {
            Guard.Against.Empty(projectNumber, nameof(projectNumber));
            Guard.Against.Null(customer, nameof(customer));
            Guard.Against.Default(invoiceCreationDate, nameof(invoiceCreationDate));
            Guard.Against.EnumOutOfRange(currency, nameof(currency));
            Guard.Against.EnumOutOfRange(vatZone, nameof(vatZone));
            Guard.Against.NegativeOrZero(netAmount, nameof(netAmount));
            Guard.Against.Negative(vatAmount, nameof(vatAmount));
            Guard.Against.NegativeOrZero(grossAmount, nameof(grossAmount));

            Id = id;
            ProjectNumber = projectNumber;
            Customer = customer;
            InvoiceCreationDate = invoiceCreationDate;
            PaymentTerms = paymentTerms;
            Currency = currency;
            VatZone = vatZone;
            NetAmount = netAmount;
            VatAmount = vatAmount;
            GrossAmount = grossAmount;
            Notes = notes;
        }
        public Invoice(
            string projectNumber,
            Customer customer,
            DateTime invoiceCreationDate,
            bool paymentTerms,
            Currency currency,
            VatZone vatZone,
            decimal netAmount,
            decimal vatAmount,
            decimal grossAmount,
            string notes)
            : this(0, projectNumber, customer, invoiceCreationDate, paymentTerms, currency, vatZone, netAmount, vatAmount, grossAmount, notes) { }

        public void AddLine(string description, int quantity, decimal unitPrice, decimal vatRate)
        {
            Guard.Against.NullOrWhiteSpace(description, nameof(description));
            Guard.Against.NegativeOrZero(quantity, nameof(quantity));
            Guard.Against.Negative(unitPrice, nameof(unitPrice));
            Guard.Against.Negative(vatRate, nameof(vatRate));

            var line = new InvoiceLine(description, quantity, unitPrice, vatRate);

            _invoiceLines.Add(line);

            RecalculateTotals();
        }

        public void SetCustomer(Customer customer)
        {
            Guard.Against.Null(customer, nameof(customer));

            if (customer.Currency != Currency || customer.VatZone != VatZone)
            {
                throw new InvalidOperationException("Customer VAT zone or currency mismatch.");
            }

            Customer = customer;
        }

        public void SetPaymentTerms(bool paymentTerms)
        {
            if (PaymentTerms == paymentTerms)
            {
                return;
            }

            PaymentTerms = paymentTerms;
        }

        public void SetNotes(string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
            {
                Notes = string.Empty;
                return;
            }

            if (notes.Length > 300)
                throw new InvalidOperationException("Notes cannot exceed 300 characters.");

            Notes = notes.Trim();
        }

        public void RecalculateTotals() 
        {
            if (_invoiceLines.Count == 0)
            {
                NetAmount = 0;
                VatAmount = 0;
                GrossAmount = 0;
                return;
            }

            NetAmount = _invoiceLines.Sum(x => x.TotalLinePrice);
            VatAmount = _invoiceLines.Sum(x => x.LineVatAmount);
            GrossAmount = NetAmount + VatAmount;
        }
    }   
}
