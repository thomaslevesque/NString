using System;
using System.Collections.Generic;
using System.Text;

namespace NString
{
    /// <summary>
    /// Encapsulates a value conversion from an input value to an output value,
    /// that will be used by <see cref="StringTemplate.Format(object, bool, IFormatProvider)"/>
    /// instead of the original input value.
    /// </summary>
    public interface IStringTemplateValueConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the value to convert.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        bool CanConvert(Type objectType);

        /// <summary>
        /// Performs the value conversion.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted output value.</returns>
        object Convert(object value);
    }
}
