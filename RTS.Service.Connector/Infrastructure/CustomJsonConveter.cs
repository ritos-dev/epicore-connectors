using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure
{
    public sealed class CustomJsonConveter : JsonConverter<TracelinkOrderSourceData>
    {
        public override TracelinkOrderSourceData? ReadJson(JsonReader reader, Type objectType, TracelinkOrderSourceData? existing, bool isExisting, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String) // check for Json value as string
            {
                var jsonString = (string)reader.Value!; 
                try
                {
                    return JsonConvert.DeserializeObject<TracelinkOrderSourceData>(jsonString);
                }
                catch
                {
                    return null;
                }
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var obj = JObject.Load(reader);
                return obj.ToObject<TracelinkOrderSourceData>();
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, TracelinkOrderSourceData? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
