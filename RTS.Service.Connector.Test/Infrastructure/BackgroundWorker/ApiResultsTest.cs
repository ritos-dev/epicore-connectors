using FluentAssertions;
using RTS.Service.Connector.Infrastructure.BackgroundWorker;

namespace RTS.Service.Connector.Test.Infrastructure.BackgroundWorker
{
    public class ApiResultTests
    {
        private readonly string _defaultData;
        private readonly string _defaultErrorMessage;

        public ApiResultTests()
        {
            _defaultData = "Test Data";
            _defaultErrorMessage = "Something went wrong";
        }

        [Fact]
        public void Success_ShouldReturnSuccessfulResult_WithDataAndNoError()
        {
            // Act
            var result = ApiResult<string>.Success(_defaultData);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(_defaultData);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void Failure_ShouldReturnFailedResult_WithErrorMessageAndDefaultData()
        {
            // Act
            var result = ApiResult<string>.Failure(_defaultErrorMessage);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull(); 
            result.ErrorMessage.Should().Be(_defaultErrorMessage);
        }

        [Fact]
        public void Failure_WithIntType_ShouldReturnDefaultIntData()
        {
            // Act
            var result = ApiResult<int>.Failure(_defaultErrorMessage);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(0); 
            result.ErrorMessage.Should().Be(_defaultErrorMessage);
        }

        [Fact]
        public void Success_WithObject_ShouldStoreObjectReference()
        {
            // Arrange
            var complexObject = new { Id = 1, Name = "Test" };

            // Act
            var result = ApiResult<object>.Success(complexObject);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeSameAs(complexObject);
        }

        [Fact]
        public void Records_ShouldBeEqual_WhenPropertiesMatch()
        {
            // Arrange
            var result1 = ApiResult<string>.Success(_defaultData);
            var result2 = ApiResult<string>.Success(_defaultData);

            // Act & Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public void Records_ShouldNotBeEqual_WhenDataDiffers()
        {
            // Arrange
            var result1 = ApiResult<string>.Success("Data A");
            var result2 = ApiResult<string>.Success("Data B");

            // Act & Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesDirectly()
        {
            // Act
            var result = new ApiResult<string>(false, _defaultData, _defaultErrorMessage);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(_defaultData);
            result.ErrorMessage.Should().Be(_defaultErrorMessage);
        }

        [Fact]
        public void Success_WithNullData_ShouldBeValid_ForReferenceTypes()
        {
            // Arrange
            string? nullData = null;

            // Act
            var result = ApiResult<string?>.Success(nullData);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void Success_WithNullData_ShouldBeValid_ForNullableValueTypes()
        {
            // Arrange
            int? nullInt = null;

            // Act
            var result = ApiResult<int?>.Success(nullInt);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Failure_ShouldAcceptNullOrEmptyMessages(string? message)
        {
            // Act
            var result = ApiResult<string>.Failure(message!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be(message);
        }
    }
}