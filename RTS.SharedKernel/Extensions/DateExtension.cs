using TimeZoneConverter;

namespace RTS.SharedKernel.Extensions
{
    public static class DateExtension
    {
        /// <summary>
        /// Compares two DateTime objects and returns the number of different hours.
        /// </summary>
        /// <param name="firstDateTime">The first DateTime object.</param>
        /// <param name="secondDateTime">The second DateTime object.</param>
        /// <returns>The number of different hours between the two DateTime objects.</returns>
        public static int DifferenceInHours(this DateTime firstDateTime, DateTime secondDateTime) => (int)(secondDateTime - firstDateTime).TotalHours;

        public static DateTime? ByUTCKind(this DateTime? dateTime) => !dateTime.HasValue
            ? null
            : DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);

        public static DateTime ByUTCKind(this DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        public static DateTime ToLocalTime(this DateTime utcTime, string timeZoneId) =>
            TimeZoneInfo.ConvertTimeFromUtc(utcTime, TZConvert.GetTimeZoneInfo(timeZoneId));

        public static DateTime ToUtcTime(this DateTime dateTime, string timeZoneId) =>
            TimeZoneInfo.ConvertTimeToUtc(dateTime, TZConvert.GetTimeZoneInfo(timeZoneId));

        public static TimeSpan GetTimeOffset(this DateTime dateTime, string timeZoneId) =>
            TZConvert.GetTimeZoneInfo(timeZoneId).GetUtcOffset(dateTime);

    }
}
