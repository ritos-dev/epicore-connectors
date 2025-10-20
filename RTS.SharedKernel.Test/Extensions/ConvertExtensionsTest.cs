using FluentAssertions;
using RTS.SharedKernel.Commons;
using RTS.SharedKernel.Extensions;
using System.Globalization;

namespace RTS.SharedKernel.Test.Extensions
{
    public class ConvertExtensionsTest
    {
        [Theory]
        [InlineData("2023-10-01T12:34:56.789Z", "2023-10-01T12:34:56.789Z")]
        [InlineData("2023-12-03T23:00:00.000Z", "2023-12-03T23:00:00.000Z")]
        [InlineData("2023-12-03T23:00:00.000", null)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void ToNullableIso8601DateTime_ShouldConvertStringToNullableDateTime(string? value, string? expected)
        {
            var result = value.ToNullableIso8601DateTime();

            if (expected == null)
            {
                result.Should().BeNull();
            }
            else
            {
                result!.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture).Should().Be(expected);
            }
        }

        [Theory]
        [InlineData("value1", TestEnumType.Value1)]
        [InlineData("VALUE2", TestEnumType.Value2)]
        [InlineData("none", TestEnumType.None)]
        public void ToEnum_ShouldConvertStringToEnum(string value, TestEnumType expected)
        {
            var result = value.ToEnum<TestEnumType>();
            result.Should().Be(expected);
        }

        [Fact]
        public void ToEnum_ShouldThrowExceptionForInvalidValue()
        {
            Action act = () => "invalid".ToEnum<TestEnumType>();
            act.Should().Throw<RtsException>().WithMessage("'invalid' is not a valid value for enum 'TestEnumType'.");
        }

        [Theory]
        [InlineData("2023-10-01T12:34:56.789Z", "2023-10-01T12:34:56.789Z")]
        public void ToIso8601DateTime_ShouldConvertStringToDateTime(string value, string expected)
        {
            var result = value.ToIso8601DateTime();
            result.ToString("yyyy-MM-ddTHH:mm:ss.fffZ").Should().Be(expected);
        }

        [Fact]
        public void ToIso8601DateTime_ShouldThrowExceptionForInvalidValue()
        {
            Action act = () => "invalid".ToIso8601DateTime();
            act.Should().Throw<RtsException>().WithMessage("'invalid' is not a valid ISO 8601 date string.");
        }

        [Theory]
        [InlineData("123", 123)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("invalid", 0)]
        public void ToIntOrDefault_ShouldConvertStringToIntOrDefault(string? value, int expected)
        {
            var result = value.ToIntOrDefault();
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("123", 123)]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("invalid", null)]
        public void ToIntOrNull_ShouldConvertStringToNullableInt(string? value, int? expected)
        {
            var result = value.ToIntOrNull();
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("123.45", 123.45)]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("invalid", null)]
        public void ToDecimalOrNull_ShouldConvertStringToNullableDecimal(string? value, double? expected)
        {
            var result = value.ToDecimalOrNull();
            result.Should().Be((decimal?)expected);
        }

        [Theory]
        [InlineData("123.45", 123.45)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("invalid", 0)]
        public void ToDecimalOrDefault_ShouldConvertStringToDecimalOrDefault(string? value, decimal expected)
        {
            var result = value.ToDecimalOrDefault();
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("1", true)]
        [InlineData("t", true)]
        [InlineData("y", true)]
        [InlineData("yes", true)]
        [InlineData("false", false)]
        [InlineData("0", false)]
        [InlineData("f", false)]
        [InlineData("n", false)]
        [InlineData("no", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("invalid", false)]
        public void ToBoolean_ShouldConvertStringToBoolean(string? value, bool expected)
        {
            var result = value.ToBoolean();
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("d3b07384-d9a0-4c9b-8f8d-3b0b0b0b0b0b", "d3b07384-d9a0-4c9b-8f8d-3b0b0b0b0b0b")]
        [InlineData(null, "00000000-0000-0000-0000-000000000000")]
        [InlineData("", "00000000-0000-0000-0000-000000000000")]
        [InlineData("invalid", "00000000-0000-0000-0000-000000000000")]
        public void ToGuidOrDefault_ShouldConvertStringToGuidOrDefault(string? value, string expected)
        {
            var result = value.ToGuidOrDefault();

            result.ToString().Should().Be(expected);
        }

        public enum TestEnumType
        {
            None,
            Value1,
            Value2
        }

        [Theory]
        [InlineData("2024-09-18 09:55:14.6587556", "2024-09-18 09:55:14.6587556")]
        [InlineData(null, null)]
        public void ByUTCKind_Should_Set_the_utc_kind_for_the_date(string? value, string? expected)
        {
            if (value == null)
            {
                DateTime? date = null;
                date.ByUTCKind().Should().BeNull();
            }
            else
            {
                var date = Convert.ToDateTime(value);
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);
                var date2 = date.ByUTCKind();
                date2.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture).Should().Be(expected);
                date2.Kind.Should().Be(DateTimeKind.Utc);
            }
        }
    }
}
