using RTS.SharedKernel.Commons;
using System.Text.Json;

namespace RTS.SharedKernel.Extensions
{
    public static class JsonFieldChecker
    {
        public static decimal GetDecimalOrThrow(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var prop))
                return prop.GetDecimal();
            throw new RtsException($"Missing or invalid field: {propertyName}");
        }

        public static int GetIntOrThrow(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var prop))
                return prop.GetInt32();
            throw new RtsException($"Missing or invalid field: {propertyName}");
        }

        public static string? GetStringOrThrow(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var prop))
                return prop.GetString();
            throw new RtsException($"Missing or invalid field: {propertyName}");
        }
    }
}
