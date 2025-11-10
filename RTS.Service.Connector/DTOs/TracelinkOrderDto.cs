using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace RTS.Service.Connector.DTOs
{
    public class TracelinkOrderDto: JsonConverter<TracelinkOrderSourceData>
    {
        [JsonProperty("order_id")]
        public string OrderId { get; init; } = string.Empty;

        [JsonProperty("company")]
        public string Company { get; init; } = string.Empty;

        [JsonProperty("number")]
        public string Number { get; init; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; init; } = string.Empty;

        [JsonProperty("description")]
        public string? Description { get; init; }

        [JsonProperty("state")]
        public string State { get; init; } = string.Empty;

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; init; }

        [JsonProperty("deadline_date")]
        public DateTime? DeadlineDate { get; init; }

        [JsonProperty("order_src_data")]
        [JsonConverter(typeof(TracelinkOrderDto))]
        public TracelinkOrderSourceData? OrderSrcData { get; init; }

        [JsonProperty("customer_id")]
        public int CustomerId { get; init; }

        [JsonProperty("update_date")]
        public DateTime? UpdatedAt { get; init; } 

        public override TracelinkOrderSourceData? ReadJson(JsonReader reader, Type objectType, TracelinkOrderSourceData? existing, bool isExisting, JsonSerializer serializer)
        {
            try
            {
            var jsonString = (string)reader.Value!;
            return JsonConvert.DeserializeObject<TracelinkOrderSourceData>(jsonString);
            }
            catch
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, TracelinkOrderSourceData? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class TracelinkOrderSourceData
    {
        [JsonProperty("number")]
        public string? Number { get; init; }
    }
}
