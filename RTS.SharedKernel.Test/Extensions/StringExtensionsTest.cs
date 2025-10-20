using RTS.SharedKernel.Extensions;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.SharedKernel.Test.Extensions
{
    public class StringExtensionsTest
    {
        [Theory]
        [InlineData("abc123", "123")]
        [InlineData("456def", "456")]
        [InlineData("78gh90", "7890")]
        [InlineData("8200 Aarhus c 12", "820012")]
        [InlineData("no numbers", "")]
        [InlineData("", "")]
        [InlineData("Aarhus N 8200", "8200")]
        [InlineData("15 Aarhus N 8200", "158200")]
        [InlineData(null, "")]
        public void ExtractNumbers_ShouldReturnCorrectNumbers(string? input, string expected)
        {
            // Act
            var result = input.ExtractNumbers();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("abc123", "123")]
        [InlineData("456def", "456")]
        [InlineData("78gh90", "78")]
        [InlineData("8200 Aarhus c 56", "8200")]
        [InlineData("Aarhus N 8200", "8200")]
        [InlineData("no numbers", "")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void ExtractFirstNumbers_ShouldReturnCorrectNumbers(string? input, string expected)
        {
            // Act
            var result = input.ExtractFirstNumbers();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
