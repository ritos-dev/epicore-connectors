using FluentAssertions;
using RTS.SharedKernel.Extensions;
using System.Globalization;

namespace RTS.SharedKernel.Test.Extensions
{
    public class DateExtensionTest
    {
        [Theory]
        [InlineData("2023-10-01T10:00:00", "2023-10-01T15:00:00", 5)]
        [InlineData("2023-10-01T10:00:00", "2023-10-02T10:00:00", 24)]
        [InlineData("2023-10-01T10:00:00", "2023-10-01T09:00:00", -1)]
        public void DifferenceInHours_ShouldReturnCorrectDifference(string firstDateTimeStr, string secondDateTimeStr, int expectedDifference)
        {
            // Arrange
            var firstDateTime = DateTime.Parse(firstDateTimeStr, CultureInfo.InvariantCulture);
            var secondDateTime = DateTime.Parse(secondDateTimeStr, CultureInfo.InvariantCulture);

            // Act
            var result = firstDateTime.DifferenceInHours(secondDateTime);

            // Assert
            result.Should().Be(expectedDifference);
        }

        [Theory]
        [InlineData("2023-10-01T10:00:00", DateTimeKind.Local, DateTimeKind.Utc)]
        [InlineData("2023-10-01T10:00:00", DateTimeKind.Unspecified, DateTimeKind.Utc)]
        [InlineData(null, DateTimeKind.Unspecified, DateTimeKind.Utc)]
        public void ByUTCKind_ShouldReturnDateTimeWithUtcKind(string? dateTimeStr, DateTimeKind initialKind, DateTimeKind expectedKind)
        {
            // Arrange
            DateTime? dateTime = (dateTimeStr != null) ? DateTime.SpecifyKind(DateTime.Parse(dateTimeStr!, CultureInfo.InvariantCulture), initialKind) : null;

            // Act
            var result = dateTime.ByUTCKind();

            // Assert
            if(dateTimeStr == null)
            {
                result.Should().BeNull();
            }
            else
            {
                result!.Value.Kind.Should().Be(expectedKind);
            }
        }

        [Theory]
        [InlineData("2024-10-25 13:12:08.173", "Romance Standard Time", "2024-10-25 11:12:08.173")]
        public void ToLocalTime_WithTimeZoneId_ShouldConvertToLocalTime(string localTime, string timeZoneId, string utcTime)
        {
            // Arrange
            var local = DateTime.Parse(localTime, CultureInfo.InvariantCulture);
            var utc = DateTime.Parse(utcTime, CultureInfo.InvariantCulture);

            // Act
            var result = utc.ToLocalTime(timeZoneId);

            var result2 = local.ToUtcTime(timeZoneId);

            // Assert
            result.Should().Be(local);
            result2.Should().Be(utc);
        }


        [Fact]
        public void Test_ToLocal()
        {
            // Arrange
            var utcTime = new DateTime(2024, 10, 25, 13, 12, 8, 173, DateTimeKind.Utc);
            var timeZoneId = "Romance Standard Time";
            // Act
            var localTime = utcTime.ToLocalTime(timeZoneId);
            // Assert
            localTime.Kind.Should().Be(DateTimeKind.Unspecified);
        }

        [Fact]
        public void Test_ToUtc()
        {
            // Arrange
            var localTime = new DateTime(2024, 10, 25, 13, 12, 8, 173,DateTimeKind.Unspecified);
            var timeZoneId = "Romance Standard Time";
            // Act
            var utcTime = localTime.ToUtcTime(timeZoneId);
            // Assert
            utcTime.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void Test_GetTimeOffset()
        {
            // Arrange
            var dateTime = new DateTime(2024, 10, 25, 13, 12, 8, 173, DateTimeKind.Utc);
            var timeZoneId = "Romance Standard Time";
            // Act
            var timeOffset = dateTime.GetTimeOffset(timeZoneId);
            // Assert
            timeOffset.Should().NotBe(TimeSpan.Zero);
        }

        [Fact]
        public void Test_TimeZoneConverter()
        {
            // Arrange
            var timeZoneIdWindows = "Romance Standard Time";
            var timeZoneIdIana = "Europe/Copenhagen";
            var date = DateTime.UtcNow;
            // Act
            var dateWindows = date.ToLocalTime(timeZoneIdWindows);
            var dateIana = date.ToLocalTime(timeZoneIdIana);
            // Assert
            dateIana.Should().Be(dateWindows);
        }
    }
}