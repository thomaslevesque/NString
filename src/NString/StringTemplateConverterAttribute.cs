using System;

namespace NString
{
    /// <summary>
    /// Indicates that the value of the property or field on which the attribute is applied 
    /// should be converted by the specified <see cref="IStringTemplateValueConverter"/> when going through
    /// the <see cref="StringTemplate.Format(object, bool, IFormatProvider)"/> method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class StringTemplateValueConverterAttribute : Attribute
    {
        /// <summary>
        /// The type of the <see cref="IStringTemplateValueConverter"/> to use.
        /// </summary>
        public Type ConverterType { get; }

        /// <summary>
        /// Constructs an instance of <see cref="StringTemplateValueConverterAttribute"/> with a converter type.
        /// </summary>
        /// <param name="converterType">The type of the <see cref="IStringTemplateValueConverter"/> to use.</param>
        public StringTemplateValueConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
