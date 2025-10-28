using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Tracelink;

public static class TracelinkParser
{
    /// <summary>
    /// Extracts an order list from a TraceLink JSON response.
    /// Handles both top-level arrays and object-wrapped arrays (e.g. "data", "orders", "result").
    /// </summary>
    public static List<OrderDto> ExtractOrders(string json)
    {
        try
        {
            var token = JToken.Parse(json);

            JArray? array = token switch
            {
                JArray directArray => directArray,
                JObject obj => obj.Properties()
                    .Select(p => p.Value)
                    .OfType<JArray>()
                    .FirstOrDefault(),
                _ => null
            };

            return array?.ToObject<List<OrderDto>>() ?? new List<OrderDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TraceLink Parser] Failed to parse order list: {ex.Message}");
            return new List<OrderDto>();
        }
    }

    /// <summary>
    /// Extracts a single order by ID from TraceLink JSON.
    /// Used when calling /tracelink/order/{orderId}.
    /// </summary>
    public static OrderDto? ExtractSingleOrder(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<OrderDto>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TraceLink Parser] Failed to parse single order: {ex.Message}");
            return null;
        }
    }
}
