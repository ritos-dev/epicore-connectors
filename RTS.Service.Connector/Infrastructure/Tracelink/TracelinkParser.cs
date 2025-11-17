using Newtonsoft.Json.Linq;
using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Tracelink
{

    // This class is responsible for moving JSON data from objects / arrays to DTO's.

    public static class TracelinkParser
    {
        // Extract order list
        public static List<TracelinkOrderListDto> ExtractOrderList(string json)
        {
            try
            {
                var root = JObject.Parse(json);
                var array = root["order"] as JArray;

                return array?.ToObject<List<TracelinkOrderListDto>>() ?? new List<TracelinkOrderListDto>();
            }
            catch
            {
                return new List<TracelinkOrderListDto>();
            }
        }

        // Extract single order
        public static TracelinkOrderDto? ExtractSingleOrder(string json)
        {
            try
            {
                var root = JObject.Parse(json);
                var orderObj = root["order"] as JObject;

                return orderObj?.ToObject<TracelinkOrderDto>();
            }
            catch
            {
                return null;
            }
        }

        // Extract customer list
        public static List<TracelinkCustomerDto> ExtractCustomerList(string json)
        {
            try
            {
                var root = JObject.Parse(json);
                var array = root["objects"] as JArray;

                return array?.ToObject<List<TracelinkCustomerDto>>() ?? new List<TracelinkCustomerDto>();
            }
            catch
            {
                return new List<TracelinkCustomerDto>();
            }
        }

        // Extract CRM list
        public static List<TracelinkCRMDto> ExtractCRM(string json)
        {
            try
            {
                var root = JObject.Parse(json);
                var array = root["objects"] as JArray;

                return array?.ToObject<List<TracelinkCRMDto>>() ?? new List<TracelinkCRMDto>();
            }
            catch
            {
                return new List<TracelinkCRMDto>();
            }
        }
    }

}