namespace NString.Properties
{
    internal static class Resources
    {
        public static string TemplateKeyNotFound(string key) =>
            $"No value found for key '{key}'.";

        public static string SubstringCountOutOfRange =>
            "The number of characters must be greater than or equal to zero, and less than or equal to the length of the string.";

        public static string NumberMustBePositiveOrZero(string argumentName) =>
            $"{argumentName} must be greater than or equal to zero.";

        public static string MaxLengthCantBeLessThan(int minValue) =>
            $"maxLength can't be less than {minValue}.";

        public static string MaxLengthCantBeLessThanLengthOfEllipsisString =>
            "maxLength can't be less than the length of ellipsisString.";
    }
}
