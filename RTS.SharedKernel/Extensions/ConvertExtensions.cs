using RTS.SharedKernel.Commons;
using System.ComponentModel;
using System.Globalization;

namespace RTS.SharedKernel.Extensions
{
    public static class ConvertExtensions
    {
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }

        public static T ToEnum<T>(this string? value) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value)) value = "none";

            var val = Array.Find(Enum.GetNames<T>(), x => x.Equals(value, StringComparison.OrdinalIgnoreCase));

            return val == null
                ? throw new RtsException(RtsException.INVALID_DATA, $"'{value}' is not a valid value for enum '{typeof(T).Name}'.")
                : Enum.Parse<T>(val);
        }
        public static T ToEnumSafe<T>(this string? value) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value)) return default;

            if (Enum.TryParse<T>(value, true, out var result))
                return result;

            return default;
        }

        public static T ToEnumFromInt<T>(this int value) where T : struct, Enum
        {

            if (!Enum.IsDefined(typeof(T), value))
                throw new RtsException(RtsException.INVALID_DATA, $"'{value}' is not a valid value for enum '{typeof(T).Name}'.");

            var result = (T)(object)value;
            
            if (result.ToString() == "none")
                throw new RtsException(RtsException.INVALID_DATA, $"'{value}' is not allowed for enum '{typeof(T).Name}'.");

            return result;
        }

        public static DateTime ToIso8601DateTime(this string? iso8601String)
        {
            if (string.IsNullOrEmpty(iso8601String))
                throw new RtsException(RtsException.INVALID_DATA, "Value cannot be null or empty.");

            var formats = new string[] { "yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-ddTHH:mm:ssZ" };

            if (!DateTime.TryParseExact(iso8601String, formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime result))
                throw new RtsException(RtsException.INVALID_DATA, $"'{iso8601String}' is not a valid ISO 8601 date string.");

            return result;
        }

        public static DateTime? ToNullableIso8601DateTime(this string? iso8601String)
        {
            var formats = new string[] { "yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-ddTHH:mm:ssZ" };

            if (string.IsNullOrEmpty(iso8601String) ||
                !DateTime.TryParseExact(iso8601String, formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime result))
                return null;

            return result;
        }

        public static int ToIntOrDefault(this string? value)
        {
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, CultureInfo.InvariantCulture, out int result))
                return 0;


            return result;
        }

        public static double ToDoubleOrDefault(this string? value)
        {
            if (string.IsNullOrEmpty(value) || !double.TryParse(value, CultureInfo.InvariantCulture, out double result))
                return 0.0;
            return result;
        }

        public static double ToDoubleOrDefault(this decimal? value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
        }

        public static double ToDoubleOrDefault(this decimal value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
        }

        public static int? ToIntOrNull(this string? value)
        {
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out int result))
                return null;

            return result;
        }

        public static decimal? ToDecimalOrNull(this string? value)
        {
            if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, CultureInfo.InvariantCulture, out decimal result))
                return null;

            return result;
        }

        public static decimal ToDecimalOrDefault(this string? value)
        {
            if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, CultureInfo.InvariantCulture, out decimal result))
                return 0m;

            return result;
        }

        public static bool ToBoolean(this string? text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            text = text.ToLower();
            string[] trueArray = ["true", "1", "t", "y", "yes"];
            string[] falseArray = ["false", "0", "f", "n", "no"];

            if (trueArray.Contains(text)) return true;
            if (falseArray.Contains(text)) return false;

            return false;
        }

        public static Guid ToGuidOrDefault(this string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Guid.Empty;
            }

            if (!Guid.TryParse(value, out Guid result))
            {
                return Guid.Empty;
            }

            return result;
        }

        public static DateOnly? ToDateOnlyOrNull(this string? text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            if (DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date)) return date;
            return null;
        }
    }
}
