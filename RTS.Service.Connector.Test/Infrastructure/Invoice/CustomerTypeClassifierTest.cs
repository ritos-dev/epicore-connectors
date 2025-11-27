using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;

using FluentAssertions;

namespace RTS.Service.Connector.Tests.Infrastructure.InvoiceSplit
{
    public class CustomerTypeClassifierTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Return_Company_When_CompanyDesc_Is_Null_Or_Empty(string? input)
        {
            var result = CustomerTypeClassifier.Classify(input);
            result.Should().Be(CustomerType.Company);
        }

        [Theory]
        [InlineData("Privat firma")]
        [InlineData("dette er en PRIVAT virksomhed")]
        [InlineData("privat, noget andet")]
        public void Should_Return_Private_When_Contains_Privat(string input)
        {
            var result = CustomerTypeClassifier.Classify(input);
            result.Should().Be(CustomerType.Private);
        }

        [Theory]
        [InlineData("Public Company")]
        [InlineData("Professionel")]
        [InlineData("business")]
        public void Should_Return_Unknown_When_No_Match(string input)
        {
            var result = CustomerTypeClassifier.Classify(input);
            result.Should().Be(CustomerType.Unknown);
        }
    }
}
