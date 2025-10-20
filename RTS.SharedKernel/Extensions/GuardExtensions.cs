using Ardalis.GuardClauses;

namespace RTS.SharedKernel.Extensions
{
    public static class GuardExtensions
    {
        private static readonly string[] AllowedImageExtensions =
            [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff"];

        public static int IntOutOfRange(this IGuardClause _, int input, int min, int max, string? message = null, Func<Exception>? exceptionCreator = null)
        {
            if (input < min || input > max)
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(nameof(input), message ?? ("Input " + nameof(input) + " was out of range"));
                }
                throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(nameof(input), message);
            }

            return input;
        }

        // gaurd against an emplty list
        public static List<T> EmptyList<T>(this IGuardClause _, List<T> input, string? message = null, Func<Exception>? exceptionCreator = null)
        {
            if (input == null || input.Count == 0)
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw exceptionCreator?.Invoke() ?? new ArgumentNullException(nameof(input), message ?? ("List " + nameof(input) + " was empty"));
                }
                throw exceptionCreator?.Invoke() ?? new ArgumentNullException(nameof(input), message);
            }
            return input;
        }

        public static string InvalidEmail(this IGuardClause guardClause, string input, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(parameterName, "Email cannot be null or whitespace.");

            try
            {
                var mail = new System.Net.Mail.MailAddress(input);
                if (mail.Address != input)
                    throw new ArgumentException("Invalid email format.", parameterName);
            }
            catch
            {
                throw new ArgumentException("Invalid email format.", parameterName);
            }

            return input;
        }

        public static string? InvalidImageExtension(this IGuardClause guardClause, string? input, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var extension = Path.GetExtension(input).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension))
                throw new ArgumentException("Invalid image file extension.", parameterName);

            return input;
        }

        public static string? WhiteSpaceChecker(this IGuardClause guardClause, string? input, string parameterName)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.Any(char.IsWhiteSpace))
                throw new ArgumentException("Input cannot contain any whitespace characters.", parameterName);

            return input;
        }

    }
}