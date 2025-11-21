using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure.Tracelink;
using Xunit;
using System.Collections.Generic;

namespace RTS.Service.Connector.Test.Tracelink
{
    public class TracelinkParserTests
    {
        private string ValidOrderListJson() => @"{ ""order"": [
            { ""order_id"": ""1"", ""number"": ""100"", ""name"": ""Order A"" },
            { ""order_id"": ""2"", ""number"": ""101"", ""name"": ""Order B"" }
        ]}";

        private string ValidSingleOrderJson() => @"{ ""order"": {
            ""company"": ""Company X"",
            ""description"": ""Desc"",
            ""state"": ""Open""
        }}";

        private string ValidCustomerListJson() => @"{ ""objects"": [
            { ""customer_id"": ""C1"", ""name"": ""Alice"" },
            { ""customer_id"": ""C2"", ""name"": ""Bob"" }
        ]}";

        private string ValidCrmListJson() => @"{ ""objects"": [
            { ""crm_id"": ""CRM1"", ""number"": ""1001"", ""name"": ""Project A"" },
            { ""crm_id"": ""CRM2"", ""number"": ""1002"", ""name"": ""Project B"" }
        ]}";

        private string ValidItemsFromCrmJson() => @"{ ""objects"": [
            { ""crm_id"": ""CRM1"", ""genobj_id"": ""OBJ1"", ""unit_order_count_f"": 10 },
            { ""crm_id"": ""CRM2"", ""genobj_id"": ""OBJ2"", ""unit_order_count_f"": 5 }
        ]}";

        private string ValidItemListJson() => @"{ ""objects"": [
            { ""object_id"": ""OBJ1"", ""brutto_price_cy"": ""100.50"" },
            { ""object_id"": ""OBJ2"", ""brutto_price_cy"": ""200.00"" }
        ]}";

        private const string InvalidJson = "invalid json";

        // Order List
        [Fact]
        public void ExtractOrderList_ValidJson_ReturnsCorrectList()
        {
            var result = TracelinkParser.ExtractOrderList(ValidOrderListJson());

            Assert.Collection(result,
                item =>
                {
                    Assert.Equal("1", item.OrderId);
                    Assert.Equal("100", item.Number);
                    Assert.Equal("Order A", item.Name);
                },
                item =>
                {
                    Assert.Equal("2", item.OrderId);
                    Assert.Equal("101", item.Number);
                    Assert.Equal("Order B", item.Name);
                });
        }

        [Fact]
        public void ExtractOrderList_InvalidJson_ReturnsEmptyList()
        {
            var result = TracelinkParser.ExtractOrderList(InvalidJson);
            Assert.Empty(result);
        }

        // Single Order
        [Fact]
        public void ExtractSingleOrder_ValidJson_ReturnsOrder()
        {
            var result = TracelinkParser.ExtractSingleOrder(ValidSingleOrderJson());
            Assert.NotNull(result);
            Assert.Equal("Company X", result.Company);
            Assert.Equal("Desc", result.Description);
            Assert.Equal("Open", result.State);
        }

        [Fact]
        public void ExtractSingleOrder_InvalidJson_ReturnsNull()
        {
            var result = TracelinkParser.ExtractSingleOrder(InvalidJson);
            Assert.Null(result);
        }

        // Customer List
        [Fact]
        public void ExtractCustomerList_ValidJson_ReturnsCorrectList()
        {
            var result = TracelinkParser.ExtractCustomerList(ValidCustomerListJson());

            Assert.Collection(result,
                item =>
                {
                    Assert.Equal("C1", item.CustomerId);
                    Assert.Equal("Alice", item.Name);
                },
                item =>
                {
                    Assert.Equal("C2", item.CustomerId);
                    Assert.Equal("Bob", item.Name);
                });
        }

        [Fact]
        public void ExtractCustomerList_InvalidJson_ReturnsEmptyList()
        {
            var result = TracelinkParser.ExtractCustomerList(InvalidJson);
            Assert.Empty(result);
        }

        // CRM List
        [Fact]
        public void ExtractCRM_ValidJson_ReturnsCorrectList()
        {
            var result = TracelinkParser.ExtractCRM(ValidCrmListJson());

            Assert.Collection(result,
                item =>
                {
                    Assert.Equal("CRM1", item.CrmId);
                    Assert.Equal("1001", item.CrmNumber);
                    Assert.Equal("Project A", item.Name);
                },
                item =>
                {
                    Assert.Equal("CRM2", item.CrmId);
                    Assert.Equal("1002", item.CrmNumber);
                    Assert.Equal("Project B", item.Name);
                });
        }

        [Fact]
        public void ExtractCRM_InvalidJson_ReturnsEmptyList()
        {
            var result = TracelinkParser.ExtractCRM(InvalidJson);
            Assert.Empty(result);
        }

        // Items from CRM
        [Fact]
        public void ExtractItemsFromCrm_ValidJson_ReturnsCorrectList()
        {
            var result = TracelinkParser.ExtractItemsFromCrm(ValidItemsFromCrmJson());

            Assert.Collection(result,
                item =>
                {
                    Assert.Equal("CRM1", item.CrmId);
                    Assert.Equal("OBJ1", item.GenObjectId);
                    Assert.Equal(10, item.ItemAmount);
                },
                item =>
                {
                    Assert.Equal("CRM2", item.CrmId);
                    Assert.Equal("OBJ2", item.GenObjectId);
                    Assert.Equal(5, item.ItemAmount);
                });
        }

        [Fact]
        public void ExtractItemsFromCrm_InvalidJson_ReturnsEmptyList()
        {
            var result = TracelinkParser.ExtractItemsFromCrm(InvalidJson);
            Assert.Empty(result);
        }

        // Item List
        [Fact]
        public void ExtractItemList_ValidJson_ReturnsCorrectList()
        {
            var result = TracelinkParser.ExtractItemList(ValidItemListJson());

            Assert.Collection(result,
                item =>
                {
                    Assert.Equal("OBJ1", item.ObjectId);
                    Assert.Equal("100.50", item.ItemPrice);
                },
                item =>
                {
                    Assert.Equal("OBJ2", item.ObjectId);
                    Assert.Equal("200.00", item.ItemPrice);
                });
        }

        [Fact]
        public void ExtractItemList_InvalidJson_ReturnsEmptyList()
        {
            var result = TracelinkParser.ExtractItemList(InvalidJson);
            Assert.Empty(result);
        }
    }
}
