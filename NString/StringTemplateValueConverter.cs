using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NString.Internal;

namespace NString
{
    /// <summary>
    /// Encapsulates a value conversion from an input value to an output value,
    /// that will be used by <see cref="StringTemplate.Format(object, bool, IFormatProvider)"/>
    /// instead of the original input value.
    /// </summary>
    public abstract class StringTemplateValueConverter<T> : IStringTemplateValueConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the value to convert.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public bool CanConvert(Type objectType)
        {
            objectType.CheckArgumentNull(nameof(objectType));
            return typeof(T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }
        /// <summary>
        /// Performs the value conversion.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted output value.</returns>
        public abstract object Convert(T value);

        object IStringTemplateValueConverter.Convert(object value)
        {
            if (value == null || value is T)
            {
                return Convert((T)value);
            }
            else
            {
                throw new NotSupportedException("Converter cannot convert the specified value.");
            }
        }
    }
}
