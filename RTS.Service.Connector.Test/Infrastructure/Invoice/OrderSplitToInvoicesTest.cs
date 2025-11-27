using FluentAssertions;
using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;

namespace RTS.Service.Connector.Test.InvoiceSplit
{
    public class OrderSplitToInvoicesTests
    {
        private readonly OrderSplitToInvoices _service;
        private readonly decimal _totalAmount;

        // Shared mock for individual invoice parts
        private readonly List<InvoicePart> _mockInvoiceParts;

        // Shared mock for the result of Split
        private readonly List<InvoicePart> _mockSplitResult;

        public OrderSplitToInvoicesTests()
        {
            _totalAmount = 1250m;

            // Mock template for InvoiceParts
            _mockInvoiceParts = new List<InvoicePart>
            {
                new() { Description = "Depositum (10%)", Percentage = 0.10m, ProductNumber = "10" },
                new() { Description = "1. Rate (50%)",   Percentage = 0.50m, ProductNumber = "15" },
                new() { Description = "2. Rate (40%)",   Percentage = 0.40m, ProductNumber = "20" }
            };

            _mockSplitResult = _mockInvoiceParts
                .Select(p => new InvoicePart
                {
                    Description = p.Description,
                    Percentage = p.Percentage,
                    NetPrice = Math.Round(_totalAmount * p.Percentage, 2, MidpointRounding.AwayFromZero),
                    ProductNumber = p.ProductNumber
                })
                .ToList();

            _service = new OrderSplitToInvoices(_mockSplitResult);
        }

        // Company and other customers tests
        [Fact]
        public void Split_Should_Return_Empty_For_Companies()
        {
            var result = _service.Split(_totalAmount, CustomerType.Company);
            result.Should().BeEmpty();
        }

        [Fact]
        public void Split_Should_Return_Empty_For_Institutes()
        {
            var result = _service.Split(_totalAmount, CustomerType.Institution);
            result.Should().BeEmpty();
        }

        [Fact]
        public void Split_Should_Return_Empty_For_Unknown_Customers()
        {
            var result = _service.Split(_totalAmount, CustomerType.Unknown);
            result.Should().BeEmpty();
        }

        // Private customer tests
        [Fact]
        public void Split_Should_Return_3_Invoices_For_Private_Customers()
        {
            var split = _service.Split(_totalAmount, CustomerType.Private);
            split.Should().HaveCount(_mockSplitResult.Count);
        }

        [Fact]
        public void Split_Should_Return_Correct_NetPrices()
        {
            var split = _service.Split(_totalAmount, CustomerType.Private);

            for (int i = 0; i < split.Count; i++)
            {
                split[i].NetPrice.Should().Be(_mockSplitResult[i].NetPrice);
            }
        }

        [Fact]
        public void Split_Should_Return_Correct_Percentages()
        {
            var split = _service.Split(_totalAmount, CustomerType.Private);

            for (int i = 0; i < split.Count; i++)
            {
                split[i].Percentage.Should().Be(_mockInvoiceParts[i].Percentage);
            }
        }

        [Fact]
        public void Split_Should_Return_Correct_Descriptions()
        {
            var split = _service.Split(_totalAmount, CustomerType.Private);

            for (int i = 0; i < split.Count; i++)
            {
                split[i].Description.Should().Be(_mockInvoiceParts[i].Description);
            }
        }

        [Fact]
        public void Split_Should_Return_Correct_ProductNumbers()
        {
            var split = _service.Split(_totalAmount, CustomerType.Private);

            for (int i = 0; i < split.Count; i++)
            {
                split[i].ProductNumber.Should().Be(_mockInvoiceParts[i].ProductNumber);
            }
        }
    }
}
