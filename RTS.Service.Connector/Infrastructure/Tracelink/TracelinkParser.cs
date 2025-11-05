using Newtonsoft.Json.Linq;
using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Tracelink;

public class TracelinkParser
{
    private readonly ILogger<ConnectorBackgroundWorker> _logger;

    public TracelinkParser(ILogger<ConnectorBackgroundWorker> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extracts an order list from a TraceLink JSON response.
    /// Handles both top-level arrays and object-wrapped arrays (e.g. "data", "orders", "result").
    /// </summary>
    public static List<TracelinkOrderDto> ExtractOrders(string json)
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

            return array?.ToObject<List<TracelinkOrderDto>>() ?? new List<TracelinkOrderDto>();
        }
        catch (Exception)
        {
            return new List<TracelinkOrderDto>();
        }
    }

    /// <summary>
    /// Extracts a single order by ID from TraceLink JSON.
    /// Used when calling /tracelink/order/{orderId}.
    /// </summary>
    public static TracelinkOrderDto? ExtractSingleOrder(string json)
    {
        try
        {
            var root = JObject.Parse(json);
            var orderToken = root["order"]; 

            if (orderToken == null)
            {
                return null;
            }

            return orderToken.ToObject<TracelinkOrderDto>();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
