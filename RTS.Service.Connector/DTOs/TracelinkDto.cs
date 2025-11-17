using Newtonsoft.Json;

namespace RTS.Service.Connector.DTOs
{
    // Order list data
    public class TracelinkOrderListDto
    {

        [JsonProperty("order_id")]
        public string OrderId { get; init; } = string.Empty;

        [JsonProperty("number")]
        public string Number { get; init; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; init; } = string.Empty;
    }

    // Order specific data
    public class TracelinkOrderDto
    {
        [JsonProperty("company")]
        public string Company { get; init; } = string.Empty;

        [JsonProperty("description")]
        public string? Description { get; init; }

        [JsonProperty("state")]
        public string State { get; init; } = string.Empty;

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; init; }

        [JsonProperty("deadline_date")]
        public DateTime? DeadlineDate { get; init; }

        [JsonProperty("update_date")]
        public DateTime? UpdatedAt { get; init; }
    }

    // Customer specific data
    public class TracelinkCustomerDto
    {
        [JsonProperty("name")]
        public string Name { get; init; } = string.Empty;

        [JsonProperty("customer_id")]
        public string CustomerId { get; init; } = string.Empty; // Specific customer id in tracelink

        [JsonProperty("address")]
        public string? CustomerAddress { get; init; }

        [JsonProperty("postalcode")]
        public string? CustomerPostalcode { get; init; }

        [JsonProperty("city")]
        public string? CustomerCity { get; init; }

        [JsonProperty("companydesc")]
        public string? CompanyType { get; init; } // Private, Public etc. 
    }

    // CRM Id for tracking projects
    public class TracelinkCRMDto
    {
        [JsonProperty("name")]
        public string Name { get; init; } = string.Empty;

        [JsonProperty("number")]
        public string? CrmNumber { get; init; } // Specific customer id in tracelink
    }
}
