using System;
using JetBrains.Annotations;

namespace NString.Internal
{
    static class ArgumentExtensions
    {
        [ContractAnnotation("value:null => halt")]
        public static void CheckArgumentNull<T>(
            [NoEnumeration] this T value,
            [InvokerParameterName] string paramName)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }
    }
}
