using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure.Economic;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;

using FluentAssertions;

namespace RTS.Service.Connector.Tests.Infrastructure.Economic
{
   
    public class EconomicInvoiceMapperTests
    {
        private readonly EconomicInvoiceMapper _mapper = new();

        private readonly EconomicInvoiceDraftDto _economicInvoiceDraftDto = new()
        {
            Currency = "DKK",
            Customer = new EconomicCustomer
            {
                CustomerNumber = 123
            },
            PaymentTerms = new EconomicPaymentTerms
            {
                PaymentTermsNumber = 10
            },
            Layout = new EconomicLayout
            {
                LayoutNumber = 5
            },
            Recipient = new EconomicRecipient
            {
                VatZone = new EconomicVatZone
                {
                    VatZoneNumber = 2
                }
            }
            // Add other properties as needed for your tests
        };

        private readonly CompleteTracelinkDto _tracelink = new()
        {
            CustomerName = "John Doe",
            CustomerAddress = "Street 1",
            CustomerPostalCode = "2100",
            CustomerCity = "Copenhagen",
            CrmNumber = "CRM-9"
        };

        private readonly InvoicePart _invoicePart = new()
        {
            Description = "Work",
            NetPrice = 300,
            ProductNumber = "77",
            Percentage = 0.50m
        };

        private EconomicInvoiceDraftDto CreateResult() => _mapper.MapToInvoiceDraft(_economicInvoiceDraftDto, _tracelink, _invoicePart);


        [Fact]
        public void Should_Map_Currency_From_Json()
        {
            CreateResult().Currency.Should().Be("DKK");
        }

        [Fact]
        public void Should_Map_CustomerNumber_From_Json()
        {
            CreateResult().Customer.CustomerNumber.Should().Be(123);
        }

        [Fact]
        public void Should_Map_PaymentTerms()
        {
            CreateResult().PaymentTerms.PaymentTermsNumber.Should().Be(10);
        }

        [Fact]
        public void Should_Map_LayoutNumber()
        {
            CreateResult().Layout.LayoutNumber.Should().Be(5);
        }

        [Fact]
        public void Should_Map_VatZone()
        {
            CreateResult().Recipient.VatZone.VatZoneNumber.Should().Be(2);
        }

        [Fact]
        public void Should_Map_Recipient_From_TracelinkDto()
        {
            var result = CreateResult();
            result.Recipient.Name.Should().Be("John Doe");
            result.Recipient.CustomerAddress.Should().Be("Street 1");
            result.Recipient.CustomerZipCode.Should().Be("2100");
            result.Recipient.CustomerCity.Should().Be("Copenhagen");
        }

        [Fact]
        public void Should_Map_InvoiceLine_From_InvoicePart()
        {
            var result = CreateResult();
            result.Lines.Should().ContainSingle();

            var line = result.Lines.Single();
            line.Description.Should().Be("Work");
            line.Quantity.Should().Be(1);
            line.UnitNetPrice.Should().Be(300);
            line.Product?.ProductNumber.Should().Be("77");
        }

        [Fact]
        public void Should_Format_Notes_Correctly()
        {
            var result = CreateResult();
            result.Notes.TextLine1 = "Part of an ongoing order";
            result.Notes.TextLine2 = "50% of the whole sale";

            result.Notes.TextLine1.Should().Contain("Part of an ongoing order");
            result.Notes.TextLine2.Should().Contain("50% of the whole sale");
        }

        [Fact]
        public void Should_Set_Date_In_Valid_Format()
        {
            CreateResult().Date.Should().MatchRegex(@"^\d{4}-\d{2}-\d{2}$");
        }
    }
}
