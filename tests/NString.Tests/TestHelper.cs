using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace NString.Tests
{
    static class TestHelper
    {
        private const string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";
        private const string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";

        public static void AssertThrowsWhenArgumentNull(Expression<Action> expr)
        {
            var realCall = expr.Body as MethodCallExpression;
            if (realCall == null)
                throw new ArgumentException("Expression body is not a method call", nameof(expr));

            var method = realCall.Method;
            var nullableContextAttribute =
                method.CustomAttributes
                .FirstOrDefault(a => a.AttributeType.FullName == NullableContextAttributeName)
                ??
                method.DeclaringType.GetTypeInfo().CustomAttributes
                .FirstOrDefault(a => a.AttributeType.FullName == NullableContextAttributeName);

            if (nullableContextAttribute is null)
                throw new InvalidOperationException($"The method '{method}' is not in a nullable enable context. Can't determine non-nullable parameters.");

            var defaultNullability = (Nullability)(byte)nullableContextAttribute.ConstructorArguments[0].Value;

            var realArgs = realCall.Arguments;
            var parameters = method.GetParameters();
            var paramIndexes = parameters
                .Select((p, i) => new { p, i })
                .ToDictionary(x => x.p.Name, x => x.i);
            var paramTypes = parameters
                .ToDictionary(p => p.Name, p => p.ParameterType);

            var nonNullableRefParams = parameters
                .Where(p => !p.ParameterType.GetTypeInfo().IsValueType && GetNullability(p, defaultNullability) == Nullability.NotNull);

            foreach (var param in nonNullableRefParams)
            {
                var paramName = param.Name;
                var args = realArgs.ToArray();
                args[paramIndexes[paramName]] = Expression.Constant(null, paramTypes[paramName]);
                var call = Expression.Call(realCall.Object, method, args);
                var lambda = Expression.Lambda<Action>(call);
                var action = lambda.Compile();
                Assert.Throws<ArgumentNullException>(paramName, action);
            }
        }

        private enum Nullability
        {
            Oblivious = 0,
            NotNull = 1,
            Nullable = 2
        }

        private static Nullability GetNullability(ParameterInfo parameter, Nullability defaultNullability)
        {
            if (parameter.ParameterType.GetTypeInfo().IsValueType)
                return Nullability.NotNull;

            var nullableAttribute = parameter.CustomAttributes
                .FirstOrDefault(a => a.AttributeType.FullName == NullableAttributeName);

            if (nullableAttribute is null)
                return defaultNullability;

            var firstArgument = nullableAttribute.ConstructorArguments.First();
            if (firstArgument.ArgumentType == typeof(byte))
            {
                var value = (byte)firstArgument.Value;
                return (Nullability)value;
            }
            else
            {
                var values = (ReadOnlyCollection<CustomAttributeTypedArgument>)firstArgument.Value;

                // Probably shouldn't happen
                if (values.Count == 0)
                    return defaultNullability;

                var value = (byte)values[0].Value;

                return (Nullability)value;
            }
        }
    }
}