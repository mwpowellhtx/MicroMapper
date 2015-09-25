namespace MicroMapper.UnitTests
{
    using System;
    using System.Collections;
    using System.Linq;
    using Should;
    using Should.Core.Exceptions;
    using Should.Core.Assertions;

    public delegate void ThrowingAction();

    public static class AssertionExtensions
    {
        public static void ShouldNotBeThrownBy(this Type exceptionType, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.NotNull(exceptionType);
                if (!exceptionType.IsInstanceOfType(ex)) return;
                throw new AssertException($"Expected no exception of type {exceptionType} to be thrown.", ex);
            }
        }

        public static void ShouldContain(this IEnumerable items, object item)
        {
            CollectionAssertExtensions.ShouldContain(items.Cast<object>(), item);
        }

        public static void ShouldBeThrownBy(this Type exceptionType, ThrowingAction action)
        {
            Exception exception = null;

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.ShouldNotBeNull();
            exception.ShouldBeType(exceptionType);
        }

        public static void ShouldBeInstanceOf<TExpectedType>(this object actual)
        {
            actual.ShouldBeType<TExpectedType>();
        }

        public static void ShouldNotBeInstanceOf<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeType<TExpectedType>();
        }

        public static T ShouldMeetSpecification<T>(this T actual, Action<T> specification)
        {
            Assert.NotNull(specification);
            specification(actual);
            return actual;
        }

        public static T ShouldCompareWith<T>(this T actual, T other, Action<T, T> comparison)
        {
            Assert.NotNull(comparison);
            comparison(actual, other);
            return actual;
        }
    }
}