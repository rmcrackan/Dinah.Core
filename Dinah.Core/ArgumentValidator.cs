using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Dinah.Core
{
    public static class ArgumentValidator
    {
        /// <summary>
        /// Used to verify that the provided argument is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument">The object that will be validated.</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <returns>If valid: return argument for convenient assignment</returns>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        public static T EnsureNotNull<T>([NotNull] T? argument, string name)
        {
            if (argument == null)
				throw new ArgumentNullException(name);
            return argument;
		}

        /// <summary>
        /// Used to verify that the provided IEnumerable object returns an enumerator that contains values.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="argument">The IEnumerable to check</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <returns>If valid: return argument for convenient assignment</returns>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <i>argument</i> is empty.</exception>
        public static IEnumerable<T> EnsureEnumerableNotNullOrEmpty<T>([NotNull] IEnumerable<T>? argument, string name)
        {
            EnsureNotNull(argument, name);
            if (!argument.Any())
				throw new ArgumentException("The value must contain one or more items.", name);
            return argument;
        }

        /// <summary>
        /// Used to verify that the provided string is not null or zero length.
        /// </summary>
        /// <param name="argument">The string that will be validated.</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <returns>If valid: return argument for convenient assignment</returns>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <i>argument</i> is an empty string.</exception>
        public static string EnsureNotNullOrEmpty([NotNull] string? argument, string name)
        {
            EnsureNotNull(argument, name);
            if (string.IsNullOrEmpty(argument))
				throw new ArgumentException("The value cannot be an empty string.", name);
            return argument;
        }

        /// <summary>
        /// Used to verify that the provided string is not null and that it doesn't contain only white space.
        /// </summary>
        /// <param name="argument">The string that will be validated.</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <returns>If valid: return argument for convenient assignment</returns>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <i>argument</i> is either an empty string or contains only white space.</exception>
        public static string EnsureNotNullOrWhiteSpace([NotNull] string? argument, string name)
        {
            EnsureNotNull(argument, name);
            if (string.IsNullOrWhiteSpace(argument))
				throw new ArgumentException("The value cannot be an empty string or contain only whitespace.", name);
            return argument;
        }

		/// <summary>
		/// Used to verify that the provided argument is greater than the minimum value. Minimum value is not valid. i.e. exclusive
		/// </summary>
		/// <param name="argument">argument to check</param>
		/// <param name="name">Name of the argument</param>
		/// <param name="minimum">Value argument must be greater than</param>
		/// <typeparam name="T">Type of the argument. Must be IComparable struct</typeparam>
		/// <returns>If valid: return argument for convenient assignment</returns>
		public static T EnsureGreaterThan<T>(T argument, string name, T minimum) where T : struct, IComparable<T>
        {
            if (argument.CompareTo(minimum) <= 0)
				throw new ArgumentException($"The provided value must be greater than {minimum}. Actual value: {argument}", name);
            return argument;
        }

		/// <summary>
		/// Used to verify that the provided argument is greater than the minimum value and less than the maximum value. Minimum and maximum values are valid. i.e. inclusive
		/// </summary>
		/// <param name="argument">argument to check</param>
		/// <param name="name">Name of the argument</param>
		/// <param name="minimum">Value argument must be greater than</param>
		/// <param name="maximum">Value argument must be less than</param>
		/// <typeparam name="T">Type of the argument. Must be IComparable struct</typeparam>
		/// <returns>If valid: return argument for convenient assignment</returns>
		public static T EnsureBetweenInclusive<T>(T argument, string name, T minimum, T maximum) where T : struct, IComparable<T>
        {
            if (argument.CompareTo(minimum) < 0 || argument.CompareTo(maximum) > 0)
                throw new ArgumentException($"The provided value must be between {minimum} and {maximum}, inclusive. Actual value: {argument}", name);
            return argument;
        }
    }
}
