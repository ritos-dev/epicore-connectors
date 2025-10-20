using CSharpFunctionalExtensions;
using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Domain.Invoices.Entities
{
    public class Customer : Entity<int>
    {
        public int CustomerId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string ProjectNumber { get; private set; } = string.Empty; // For tracking projects and recipients

        public string Address { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string ZipCode { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;

        public Currency Currency { get; private set; }
        public string CustomerVatNumber { get; private set; } = string.Empty;
        public VatZone VatZone { get; private set; }
        public bool PaymentTerms { get; private set; } // True --> Lets economic set payment terms after draft is created

        public CustomerType CustomerType { get; private set; }

        public Customer() { }

        public Customer(int id, int customerId, string name, string projectNumber, string address, string city, string zipCode, string country, Currency currency, string customerVatNumber, VatZone vatZone, bool paymentTerms, CustomerType customerType)
        {
            Id = id;
            CustomerId = customerId;
            Name = name;
            ProjectNumber = projectNumber;
            Address = address;
            City = city;
            ZipCode = zipCode;
            Country = country;
            Currency = currency;
            CustomerVatNumber = customerVatNumber;
            VatZone = vatZone;
            PaymentTerms = paymentTerms;
            CustomerType = customerType;
        }
        public Customer(int customerId, string name, string projectNumber, string address, string city, string zipCode, string country, Currency currency, string customerVatNumber, VatZone vatZone, bool paymentTerms, CustomerType customerType)
            : this(0, customerId, name, projectNumber, address, city, zipCode, country, currency, customerVatNumber, vatZone, paymentTerms, customerType)
        {
        }
    }
}
