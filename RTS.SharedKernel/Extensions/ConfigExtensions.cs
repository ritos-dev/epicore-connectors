using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text;

namespace RTS.SharedKernel.Extensions
{
    public static class ConfigExtensions
    {
        public static string GetCnnString(this IConfiguration configuration, string connectionName) =>
            configuration.GetConnectionString(connectionName) ?? configuration[connectionName] ?? string.Empty;

        public static string GetCnnString(this IConfiguration configuration) => configuration.GetCnnString("DefaultConnection");

        public static string GetSettings(this IConfiguration configuration, string section, string subSection) =>
            Environment.GetEnvironmentVariable($"{section}__{subSection}") ?? configuration[$"{section}:{subSection}"] ?? string.Empty;

        public static int GetIntSettings(this IConfiguration configuration, string section, string subSection)
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(subSection))
                return 0;

            if (int.TryParse(Environment.GetEnvironmentVariable($"{section}__{subSection}") ?? configuration[$"{section}:{subSection}"], CultureInfo.InvariantCulture, out int result))
                return result;

            return 0;
        }

        public static double GetDoubleSettings(this IConfiguration configuration, string section, string subSection)
        {
            try
            {
                return Convert.ToDouble(Environment.GetEnvironmentVariable($"{section}__{subSection}") ?? configuration[$"{section}:{subSection}"], CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static bool GetBoolSettings(this IConfiguration configuration, string section, string subSection)
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(subSection))
                return false;

            if (bool.TryParse(Environment.GetEnvironmentVariable($"{section}__{subSection}") ?? configuration[$"{section}:{subSection}"], out bool result))
                return result;

            return false;
        }

        public static byte[] GetByteSettings(this IConfiguration configuration, string section, string subSection) =>
            Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable($"{section}__{subSection}") ?? configuration.GetSettings(section, subSection));

        public static Guid GetGuidSettings(this IConfiguration configuration, string section, string subSection)
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(subSection))
                return Guid.Empty;

            if (Guid.TryParse(Environment.GetEnvironmentVariable($"{section}__{subSection}") ?? configuration[$"{section}:{subSection}"], out Guid result))
                return result;

            return Guid.Empty;
        }

        public static string[] GetStringArraySettings(this IConfiguration configuration, string section, string subSection)
        {
            var value = Environment.GetEnvironmentVariable($"{section}__{subSection}") ?? configuration[$"{section}:{subSection}"];
            return string.IsNullOrEmpty(value) ? [] : value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }   
    }
}