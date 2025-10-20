using System.Text;

namespace RTS.SharedKernel.Extensions
{
    public static class StringExtensions
    {
        public static string ExtractNumbers(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var numbers = new StringBuilder();
            numbers.Append(input.Where(char.IsDigit).ToArray());
            return numbers.ToString();
        }

        public static string ExtractFirstNumbers(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            bool numberFound = false;
            var numbers = new StringBuilder();
            foreach (var c in input)
            {
                if (numberFound && !char.IsDigit(c))
                    break;

                if (char.IsDigit(c))
                {
                    numbers.Append(c);
                    numberFound = true;
                }
            }

            return numbers.ToString();
        }
    }
}
