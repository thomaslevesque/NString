using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NString.Internal;
using NString.Properties;

namespace NString
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Indicates whether the specified string is null or an Empty string.
        /// </summary>
        /// <param name="s">the string to test</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        /// <remarks>This is just a shortcut for <see cref="String.IsNullOrEmpty"/>, allowing it to be used as an extension method.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("s:null=>true")]
        [Pure]
        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="s">the string to test</param>
        /// <returns>true if the value parameter is null or String.Empty, or if value consists exclusively of white-space characters.</returns>
        /// <remarks>This is just a shortcut for <see cref="String.IsNullOrWhiteSpace"/>, allowing it to be used as an extension method.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("s:null=>true")]
        [Pure]
        public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// Concatenates all strings in the collection, using the specified separator between each string.
        /// </summary>
        /// <param name="values">A collection that contains the strings to concatenate.</param>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
        /// <exception cref="ArgumentNullException">values is null.</exception>
        /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns String.Empty.</returns>
        [Pure]
        public static string Join([NotNull] this IEnumerable<string> values, string? separator = null)
        {
            values.CheckArgumentNull(nameof(values));
            return string.Join(separator, values);
        }

        /// <summary>
        /// Enumerates all lines in a string.
        /// </summary>
        /// <param name="s">The string whose lines are to be enumerated.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <returns>An enumerable sequence of lines in this string.</returns>
        [Pure]
        public static IEnumerable<string> GetLines([NotNull] this string s)
        {
            s.CheckArgumentNull(nameof(s));
            return GetLinesIterator(s);
        }

        private static IEnumerable<string> GetLinesIterator(string s)
        {
            using (StringReader reader = new StringReader(s))
            {
                while (reader.ReadLine() is string line)
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Returns a string containing a specified number of characters from the left side of a string.
        /// </summary>
        /// <param name="s">The string from which the leftmost characters are returned.</param>
        /// <param name="count">The number of characters to return.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">count is less than 0 or greater than the length of the string.</exception>
        /// <returns>A string containing a specified number of characters from the left side of a string.</returns>
        [Pure]
        public static string Left([NotNull] this string s, int count)
        {
            s.CheckArgumentNull(nameof(s));
            count.CheckArgumentOutOfRange(nameof(count), 0, s.Length, Resources.SubstringCountOutOfRange);
            return s.Substring(0, count);
        }

        /// <summary>
        /// Returns a string containing a specified number of characters from the right side of a string.
        /// </summary>
        /// <param name="s">The string from which the rightmost characters are returned.</param>
        /// <param name="count">The number of characters to return.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">count is less than 0 or greater than the length of the string.</exception>
        /// <returns>A string containing a specified number of characters from the right side of a string.</returns>
        [Pure]
        public static string Right([NotNull] this string s, int count)
        {
            s.CheckArgumentNull(nameof(s));
            count.CheckArgumentOutOfRange(nameof(count), 0, s.Length, Resources.SubstringCountOutOfRange);
            return s.Substring(s.Length - count, count);
        }

        /// <summary>
        /// Returns a string truncated to the specified number of characters.
        /// </summary>
        /// <param name="s">The string to truncate.</param>
        /// <param name="count">The maximum number of characters to return.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">count is less than 0.</exception>
        /// <returns>A string tuncated to the specified number of characters.</returns>
        [Pure]
        public static string Truncate([NotNull] this string s, int count)
        {
            s.CheckArgumentNull(nameof(s));
            count.CheckArgumentOutOfRange(
                nameof(count), 0, int.MaxValue,
                Resources.NumberMustBePositiveOrZero(nameof(count)));
            if (count > s.Length)
                return s;
            return s.Substring(0, count);
        }

        /// <summary>
        /// Capitalizes a string by making its first character uppercase, using the current culture.
        /// </summary>
        /// <param name="s">The string to capitalize.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <returns>The capitalized string.</returns>
        public static string Capitalize([NotNull] this string s)
        {
            return Capitalize(s, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Capitalizes a string by making its first character uppercase, using the specified culture.
        /// </summary>
        /// <param name="s">The string to capitalize.</param>
        /// <param name="culture">The culture to use when making the first character uppercase.</param>
        /// <exception cref="ArgumentNullException">s or culture is null.</exception>
        /// <returns>The capitalized string.</returns>
        [Pure]
        public static string Capitalize([NotNull] this string s, [NotNull] CultureInfo culture)
        {
            s.CheckArgumentNull(nameof(s));
            culture.CheckArgumentNull(nameof(culture));
            if (s.Length == 0)
                return s;
            var chars = s.ToCharArray();
            chars[0] = culture.TextInfo.ToUpper(chars[0]);
            return new string(chars);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char CharAt(this string s, int index)
        {
            return index < s.Length ? s[index] : '\0';
        }

        /// <summary>
        /// Checks if a string matches the specified wildcard pattern.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <param name="pattern">The pattern against which the string is compared.</param>
        /// <param name="caseSensitive">true to perform a case-sensitive check; otherwise, false. The default is true.</param>
        /// <exception cref="ArgumentNullException">s or pattern is null.</exception>
        /// <returns>true if the string matches the pattern; otherwise, false.</returns>
        /// <remarks>The pattern can contain wildcards such as '*' (any number of characters) or '?' (exactly one character).
        /// This method is not culture-sensitive.</remarks>
        [Pure]
        public static bool MatchesWildcard([NotNull] this string s, string pattern, bool caseSensitive = true)
        {
            s.CheckArgumentNull(nameof(s));
            pattern.CheckArgumentNull(nameof(pattern));
            if (!caseSensitive)
            {
                s = s.ToUpperInvariant();
                pattern = pattern.ToUpperInvariant();
            }

            int it = 0;
            while (s.CharAt(it) != 0 &&
                   pattern.CharAt(it) != '*')
            {
                if (pattern.CharAt(it) != s.CharAt(it) && pattern.CharAt(it) != '?')
                    return false;
                it++;
            }

            int cp = 0;
            int mp = 0;
            int ip = it;

            while (s.CharAt(it) != 0)
            {
                if (pattern.CharAt(ip) == '*')
                {
                    if (pattern.CharAt(++ip) == 0)
                        return true;
                    mp = ip;
                    cp = it + 1;
                }
                else if (pattern.CharAt(ip) == s.CharAt(it) || pattern.CharAt(ip) == '?')
                {
                    ip++;
                    it++;
                }
                else
                {
                    ip = mp;
                    it = cp++;
                }
            }

            while (pattern.CharAt(ip) == '*')
            {
                ip++;
            }
            return pattern.CharAt(ip) == 0;
        }

        /// <summary>
        /// Truncates a string to the specified length, replacing the last characters with an ellipsis (three dots)
        /// </summary>
        /// <param name="s">The string to truncate.</param>
        /// <param name="maxLength">The maximum desired length.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxLength is less than 3.</exception>
        /// <returns>The truncated string.</returns>
        [Pure]
        public static string Ellipsis([NotNull] this string s, int maxLength)
        {
            const string ellipsisString = "...";
            if (maxLength < ellipsisString.Length)
                throw new ArgumentOutOfRangeException(Resources.MaxLengthCantBeLessThan(ellipsisString.Length));
            return s.Ellipsis(maxLength, ellipsisString);
        }

        /// <summary>
        /// Truncates a string to the specified length, replacing the last characters with the specified ellipsis string.
        /// </summary>
        /// <param name="s">The string to truncate.</param>
        /// <param name="maxLength">The maximum desired length.</param>
        /// <param name="ellipsisString">The string to use as an ellipsis.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxLength is less than the length of ellipsisString.</exception>
        /// <returns>The truncated string.</returns>
        [Pure]
        public static string Ellipsis([NotNull] this string s, int maxLength, string ellipsisString)
        {
            s.CheckArgumentNull(nameof(s));
            ellipsisString.CheckArgumentNull(nameof(ellipsisString));
            if (maxLength < ellipsisString.Length)
                throw new ArgumentOutOfRangeException(Resources.MaxLengthCantBeLessThanLengthOfEllipsisString);

            if (s.Length <= maxLength)
                return s;

            return s.Substring(0, maxLength - ellipsisString.Length) + ellipsisString;
        }

        ///<summary>
        /// Checks if the specified string contains the specified substring, using the specified comparison type.
        ///</summary>
        ///<param name="s">The string in which to seek a substring.</param>
        ///<param name="subString">The string to seek.</param>
        ///<param name="comparisonType">The type of comparison to use.</param>
        ///<exception cref="ArgumentNullException">s or subString is null.</exception>
        ///<exception cref="ArgumentOutOfRangeException">comparisonType is not a valid StringComparison value.</exception>
        ///<returns>true si <c>s</c> contains <c>subString</c>, or if <c>subString</c> is an empty string; otherwise, false.</returns>
        public static bool Contains([NotNull] this string s, string subString, StringComparison comparisonType)
        {
            s.CheckArgumentNull(nameof(s));
            subString.CheckArgumentNull(nameof(subString));
            comparisonType.CheckArgumentInEnum(nameof(comparisonType));
            return s.IndexOf(subString, comparisonType) >= 0;
        }

        /// <summary>
        /// Replaces a single character at the specified position with the specified replacement character.
        /// </summary>
        /// <param name="s">The string in which a character will be replaced.</param>
        /// <param name="index">The position of the character to replace.</param>
        /// <param name="newChar">The replacement character.</param>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">index doesn't refer to a valid position within the string.</exception>
        /// <returns>The string with the replaced character.</returns>
        [Pure]
        public static string ReplaceAt([NotNull] this string s, int index, char newChar)
        {
            s.CheckArgumentNull(nameof(s));
            index.CheckArgumentOutOfRange(nameof(index), 0, s.Length - 1);

            var chars = s.ToCharArray();
            chars[index] = newChar;
            return new string(chars);
        }

        private static IEnumerable<string> GetTextElements(this string s)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
                yield return (string) enumerator.Current;
        }

        /// <summary>
        /// Reverses the specified string.
        /// </summary>
        /// <param name="s">The string to reverse</param>
        /// <returns>The string with its characters reversed.</returns>
        /// <exception cref="ArgumentNullException">s is null</exception>
        /// <remarks>This methods works on full UTF-16 code points, not code units. This means that it will correctly handle surrogate pairs such as emoji or some asian characters.</remarks>
        [Pure]
        public static string Reverse([NotNull] this string s)
        {
            s.CheckArgumentNull(nameof(s));
            var textElements = s.GetTextElements();
            return string.Concat(textElements.Reverse());
        }
    }
}
