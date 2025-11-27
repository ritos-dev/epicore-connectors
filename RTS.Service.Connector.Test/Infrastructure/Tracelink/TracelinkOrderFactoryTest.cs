using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure.Tracelink;

using FluentAssertions;

namespace RTS.Service.Connector.Test.Tracelink
{
    public class TracelinkOrderFactoryTests
    {
        private readonly TracelinkOrderListDto _orderList = new()
        {
            OrderId = "1",
            Number = "100",
            Name = "Order A"
        };

        private readonly TracelinkOrderDto _orderDetails = new()
        {
            Description = "Test",
            State = "Open"
        };

        private readonly TracelinkCustomerDto _customer = new()
        {
            CustomerId = "C1",
            CustomerCity = "Copenhagen"
        };

        private readonly TracelinkCRMDto _crm = new()
        {
            CrmId = "CRM1",
            CrmNumber = "1001"
        };

        private CompleteTracelinkDto CreateResult() =>
            TracelinkOrderFactory.Create(_orderList, _orderDetails, _customer, _crm);


        [Fact]
        public void Should_MapOrderListData() =>
            CreateResult().Should().Match<CompleteTracelinkDto>(r =>
                r.OrderId == "1" &&
                r.OrderNumber == "100" &&
                r.CustomerName == "Order A");

        [Fact]
        public void Should_MapOrderDetails() =>
            CreateResult().Should().Match<CompleteTracelinkDto>(r =>
                r.Description == "Test" &&
                r.State == "Open");

        [Fact]
        public void Should_MapCustomerData() =>
            CreateResult().Should().Match<CompleteTracelinkDto>(r =>
                r.CustomerId == "C1" &&
                r.CustomerCity == "Copenhagen");

        [Fact]
        public void Should_MapCrmData() =>
            CreateResult().Should().Match<CompleteTracelinkDto>(r =>
                r.CrmId == "CRM1" &&
                r.CrmNumber == "1001");

        [Fact]
        public void Should_Initialize_Items_AsEmpty() =>
            CreateResult().Items.Should().BeEmpty();
    }
}
