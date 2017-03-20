using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using NString.Internal;
using NString.Properties;

namespace NString
{
    /// <summary>
    /// Provides a string template mechanism with named placeholders that can be used to format objects.
    /// </summary>
    /// <example><![CDATA[static void Main()
    /// {
    ///     var joe = new Person { Name = "Joe", DateOfBirth = new DateTime(1980, 6, 22) };
    ///     string text = StringTemplate.Format("{Name} was born on {DateOfBirth:D}", joe);
    ///     Console.WriteLine(text); // Prints "Joe was born on Sunday, 22 June 1980"
    /// }
    /// 
    /// class Person
    /// {
    ///     public string Name { get; set; }
    ///     public DateTime DateOfBirth { get; set; }
    /// }]]>
    /// </example>
    /// <remarks>The template syntax is similar to the one used in String.Format, except that indexes are replaced by names.</remarks>
    public class StringTemplate
    {
        private readonly string _template;
        private readonly string _templateWithIndexes;
        private readonly IList<string> _placeholders;

        private StringTemplate(string template)
        {
            template.CheckArgumentNull(nameof(template));
            _template = template;
            ParseTemplate(out _templateWithIndexes, out _placeholders);
        }

        /// <summary>
        /// Parses the provided string into a StringTemplate instance. Parsed templates are cached, so calling this method twice
        /// with the same argument will return the same StringTemplate instance.
        /// </summary>
        /// <param name="template">string representation of the template</param>
        /// <returns>A StringTemplate instance that can be used to format objects.</returns>
        /// <remarks>The template syntax is similar to the one used in String.Format, except that indexes are replaced by names.</remarks>
        public static StringTemplate Parse([NotNull] string template)
        {
            return GetTemplate(template);
        }

        /// <summary>
        /// Converts a string to a StringTemplate.
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <returns>A StringTemplate using the converted string</returns>
        public static implicit operator StringTemplate([NotNull] string s)
        {
            s.CheckArgumentNull(nameof(s));
            return GetTemplate(s);
        }

        /// <summary>
        /// Returns a string representation of this StringTemplate.
        /// </summary>
        /// <returns>The string representation of this StringTemplate</returns>
        public override string ToString()
        {
            return _template;
        }

        /// <summary>
        /// Replaces the template's placeholders with the values from the specified dictionary.
        /// </summary>
        /// <param name="values">A dictionary containing values for each placeholder in the template</param>
        /// <param name="throwOnMissingValue">Indicates whether or not to throw an exception if a value is missing for a placeholder.
        /// If this parameter is false and no value is found, the placeholder is left as is in the formatted string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">throwOnMissingValue is true and no value was found in the dictionary for a placeholder</exception>
        public string Format([NotNull] IDictionary<string, object> values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            values.CheckArgumentNull(nameof(values));

            object[] array = new object[_placeholders.Count];
            for (int i = 0; i < _placeholders.Count; i++)
            {
                string key = _placeholders[i];
                object value;
                if (!values.TryGetValue(key, out value))
                {
                    if (throwOnMissingValue)
                        throw new KeyNotFoundException(Resources.TemplateKeyNotFound(key));
                    value = $"{{{key}}}";
                }
                array[i] = value;
            }
            return string.Format(formatProvider, _templateWithIndexes, array);
        }

        /// <summary>
        /// Replaces the template's placeholders with the values from the specified object.
        /// </summary>
        /// <param name="values">An object containing values for the placeholders. For each placeholder, this method looks for a
        /// corresponding property of field in this object.</param>
        /// <param name="throwOnMissingValue">Indicates whether or not to throw an exception if a value is missing for a placeholder.
        /// If this parameter is false and no value is found, the placeholder is left as is in the formatted string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">throwOnMissingValue is true and no value was found in the dictionary for a placeholder</exception>
        public string Format([NotNull] object values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            values.CheckArgumentNull(nameof(values));
            return Format(MakeDictionary(values), throwOnMissingValue, formatProvider);
        }

        /// <summary>
        /// Replaces the specified template's placeholders with the values from the specified dictionary.
        /// </summary>
        /// <param name="template">The template to use to format the values.</param>
        /// <param name="values">A dictionary containing values for each placeholder in the template</param>
        /// <param name="throwOnMissingValue">Indicates whether or not to throw an exception if a value is missing for a placeholder.
        /// If this parameter is false and no value is found, the placeholder is left as is in the formatted string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">throwOnMissingValue is true and no value was found in the dictionary for a placeholder</exception>
        public static string Format([NotNull] string template, [NotNull] IDictionary<string, object> values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            return GetTemplate(template).Format(values, throwOnMissingValue, formatProvider);
        }

        /// <summary>
        /// Replaces the specified template's placeholders with the values from the specified object.
        /// </summary>
        /// <param name="template">The template to use to format the values.</param>
        /// <param name="values">An object containing values for the placeholders. For each placeholder, this method looks for a
        /// corresponding property of field in this object.</param>
        /// <param name="throwOnMissingValue">Indicates whether or not to throw an exception if a value is missing for a placeholder.
        /// If this parameter is false and no value is found, the placeholder is left as is in the formatted string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">throwOnMissingValue is true and no value was found in the dictionary for a placeholder</exception>
        public static string Format([NotNull] string template, [NotNull] object values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            return GetTemplate(template).Format(values, throwOnMissingValue, formatProvider);
        }

        /// <summary>
        /// Clears the cached templates and property getters.
        /// </summary>
        public static void ClearCache()
        {
            _templateCache.Clear();
            _gettersCache.Clear();
        }

        private static readonly Regex _regex = new Regex(@"(?<open>{+)(?<key>\w+)\s*(?<alignment>,\s*-?\d+)?\s*(?<format>:[^}]+)?(?<close>}+)");
        private void ParseTemplate(out string templateWithIndexes, out IList<string> placeholders)
        {
            var tmp = new List<string>();
            MatchEvaluator evaluator = m =>
            {
                string open = m.Groups["open"].Value;
                string close = m.Groups["close"].Value;
                string key = m.Groups["key"].Value;
                string alignment = m.Groups["alignment"].Value;
                string format = m.Groups["format"].Value;

                if (open.Length % 2 == 0)
                    return m.Value;

                open = RemoveLastChar(open);
                close = RemoveLastChar(close);

                if (!tmp.Contains(key))
                {
                    tmp.Add(key);
                }

                int index = tmp.IndexOf(key);
                return $"{open}{{{index}{alignment}{format}}}{close}";
            };
            templateWithIndexes = _regex.Replace(_template, evaluator);
            placeholders = tmp.AsReadOnly();
        }

        private static string RemoveLastChar(string str)
        {
            if (str.Length > 1)
                return str.Substring(0, str.Length - 1);
            return string.Empty;
        }

        private IDictionary<string, object> MakeDictionary(object obj)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (string name in _placeholders)
            {
                object value;
                if (TryGetMemberValue(obj, name, out value))
                    dict.Add(name, value);
            }
            return dict;
        }

        private static bool TryGetMemberValue(object obj, string memberName, out object value)
        {
            Type type = obj.GetType();
            Func<object, object> getter = GetGetterFromCache(type, memberName);
            if (getter != null)
            {
                value = getter(obj);
                return true;
            }
            value = null;
            return false;
        }

        #region Cache

        private static readonly Cache<string, StringTemplate> _templateCache = new Cache<string, StringTemplate>();

        private static StringTemplate GetTemplate(string template)
        {
            template.CheckArgumentNull(nameof(template));
            return _templateCache.GetOrAdd(template, () => new StringTemplate(template));
        }

        private static readonly Cache<Type, Cache<string, Func<object, object>>> _gettersCache =
            new Cache<Type, Cache<string, Func<object, object>>>();

        private static Func<object, object> GetGetterFromCache(Type type, string memberName)
        {
            var typeGetters = _gettersCache.GetOrAdd(type, () => new Cache<string, Func<object, object>>());
            var getter = typeGetters.GetOrAdd(memberName, () => CreateGetter(type, memberName));
            return getter;
        }

        private static Func<object, object> CreateGetter(Type type, string memberName)
        {
            MemberInfo member = null;
            while (type != null)
            {
                var info = type.GetTypeInfo();
                var prop = info.GetDeclaredProperty(memberName);
                if (prop != null && prop.CanRead && prop.GetMethod.IsPublic)
                {
                    member = prop;
                    break;
                }
                var field = info.GetDeclaredField(memberName);
                if (field != null && field.IsPublic)
                {
                    member = field;
                    break;
                }
                type = info.BaseType;
            }
            if (member == null)
                return null;
            var param = Expression.Parameter(typeof(object), "x");
            var memberAccess = Expression.MakeMemberAccess(Expression.Convert(param, type), member);
            Expression body = memberAccess;
            if (memberAccess.Type.GetTypeInfo().IsValueType)
                body = Expression.Convert(memberAccess, typeof(object));

            var expr = Expression.Lambda<Func<object, object>>(body, param);
            return expr.Compile();
        }

        #endregion
    }
}
