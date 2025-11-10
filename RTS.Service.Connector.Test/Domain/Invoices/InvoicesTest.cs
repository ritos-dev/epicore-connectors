/*using FluentAssertions;
using RTS.SharedKernel.Extensions;
using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Domain.Invoices;
using RTS.Service.Connector.Domain.Invoices.Entities;
using System.Diagnostics.Eventing.Reader;

namespace RTS.Service.Connector.Test.Domain.Invoices
{
    public class InvoicesTest
    {
        private static Customer CreateCustomer()
        {
            return new Customer(
                id: 1,
                customerId: 100,
                name: "Test Customer",
                projectNumber: "PN123",
                address: "123 Test St",
                city: "City",
                zipCode: "0000",
                country: "Denmark",
                currency: Currency.DKK,
                customerVatNumber: "DK10101010",
                vatZone: VatZone.Domestic,
                paymentTerms: true,
                customerType: CustomerType.Company
                );
        }

        private static Invoice CreateInvoice()
        {
            var customer = CreateCustomer();

            return new Invoice(
                id: 1,
                projectNumber: "P-001",
                customer: customer,
                invoiceCreationDate: DateTime.UtcNow,
                paymentTerms: true,
                currency: Currency.DKK,
                vatZone: VatZone.Domestic,
                netAmount: 1000m,
                vatAmount: 250m,
                grossAmount: 1250m,
                notes: "Notes"
                );
        }

        [Fact]
        public void CanCreateInvoice_WithValidData()
        {
            //Arrange
            var invoice = CreateInvoice();

            //Act 
            var result = invoice;

            //Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void InvalidInvoice_ThrowsException_WhenProjectNumberIsEmpty()
        {
            //Arrange
            var invoice = CreateInvoice();

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                "",
                invoice.Customer,
                invoice.InvoiceCreationDate,
                invoice.PaymentTerms,
                invoice.Currency,
                invoice.VatZone,
                invoice.NetAmount,
                invoice.VatAmount,
                invoice.GrossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*projectNumber*");
        }

        [Fact]
        public void InvalidInvoice_ThrowsException_WhenCustomerIsNull()
        {
            //Arrange
            var invalidCustomer = (Customer)null!;
            var invoice = CreateInvoice();

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                invoice.ProjectNumber,
                invalidCustomer,
                invoice.InvoiceCreationDate,
                invoice.PaymentTerms,
                invoice.Currency,
                invoice.VatZone,
                invoice.NetAmount,
                invoice.VatAmount,
                invoice.GrossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*customer*");
        }

        [Fact]
        public void Invoice_UsesDefaultDate_WhenDateIsNull()
        {
            //Arrange
            var invoice = CreateInvoice();

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                invoice.ProjectNumber,
                invoice.Customer,
                null,
                invoice.PaymentTerms,
                invoice.Currency,
                invoice.VatZone,
                invoice.NetAmount,
                invoice.VatAmount,
                invoice.GrossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*invoiceCreationDate*");
        }

        [Fact]
        public void InvalidInvoice_ThrowsExeption_WhenCurrencyIsWrong()
        {
            //Arrange
            var invoice = CreateInvoice();
            var currency = (Currency)999;

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                invoice.ProjectNumber,
                invoice.Customer,
                invoice.InvoiceCreationDate,
                invoice.PaymentTerms,
                currency,
                invoice.VatZone,
                invoice.NetAmount,
                invoice.VatAmount,
                invoice.GrossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*currency*");
        }

        [Fact]
        public void InvalidInvoice_ThrowsExeption_WhenVatZoneIsWrong()
        {
            //Arrange
            var invoice = CreateInvoice();
            var vatZone = (VatZone)999;

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                invoice.ProjectNumber,
                invoice.Customer,
                invoice.InvoiceCreationDate,
                invoice.PaymentTerms,
                invoice.Currency,
                vatZone,
                invoice.NetAmount,
                invoice.VatAmount,
                invoice.GrossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*vatZone*");
        }

        [Fact]
        public void InvalidInvoice_ThrowsExeption_WhenNetAmountIsWrong()
        {
            //Arrange
            var invoice = CreateInvoice();
            var netAmount = -10m;

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                invoice.ProjectNumber,
                invoice.Customer,
                invoice.InvoiceCreationDate,
                invoice.PaymentTerms,
                invoice.Currency,
                invoice.VatZone,
                netAmount,
                invoice.VatAmount,
                invoice.GrossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*netAmount*");
        }

        [Fact]
        public void InvalidInvoice_ThrowsExeption_WhenVatAmountIsWrong()
        {
            //Arrange
            var invoice = CreateInvoice();
            var vatAmount = -10m;

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                invoice.ProjectNumber,
                invoice.Customer,
                invoice.InvoiceCreationDate,
                invoice.PaymentTerms,
                invoice.Currency,
                invoice.VatZone,
                invoice.NetAmount,
                vatAmount,
                invoice.GrossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*vatAmount*");
        }

        [Fact]
        public void InvalidInvoice_ThrowsExeption_WhenGrossAmountIsWrong()
        {
            //Arrange
            var invoice = CreateInvoice();
            var grossAmount = -10m;

            //Act
            var invalidInvoice = () => new Invoice(
                invoice.Id,
                invoice.ProjectNumber,
                invoice.Customer,
                invoice.InvoiceCreationDate,
                invoice.PaymentTerms,
                invoice.Currency,
                invoice.VatZone,
                invoice.NetAmount,
                invoice.VatAmount,
                grossAmount,
                invoice.Notes
                );

            //Assert
            invalidInvoice.Should().Throw<ArgumentException>().WithMessage("*grossAmount*");
        }
    }
}*/
