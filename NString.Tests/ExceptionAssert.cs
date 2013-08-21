using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NString.Tests
{
    static class ExceptionAssert
    {
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
                Assert.Fail("Expected exception of type '{0}', but no exception occurred", typeof(TException));
            }
            catch (TException)
            {
            }
            catch(Exception ex)
            {
                Assert.Fail("Expected exception of type '{0}', but got '{1}' instead", typeof(TException), ex.GetType());
            }
        }

        public static void Throws(Action action)
        {
            try
            {
                action();
                Assert.Fail("Expected exception, but none occurred");
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }
    }
}
