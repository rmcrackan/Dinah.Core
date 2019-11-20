using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dinah.Core
{
    public static class ArgumentValidator
    {
        /// <summary>
        /// Used to verify that the provided argument is not null.
        /// </summary>
        /// <param name="argument">The object that will be validated.</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        public static void EnsureNotNull(object argument, string name)
        {
            if (argument == null)
				throw new ArgumentNullException(name);
		}

        /// <summary>
        /// Used to verify that the provided IEnumerable object returns an enumerator that contains values.
        /// </summary>
        /// <param name="argument">The IEnumerable to check</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <i>argument</i> is empty.</exception>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        public static void EnsureEnumerableNotNullOrEmpty<T>(IEnumerable<T> argument, string name)
        {
            EnsureNotNull(argument, name);
            if (!argument.Any())
				throw new ArgumentException("The value must contain one or more items.", name);
		}

        /// <summary>
        /// Used to verify that the provided string is not null or zero length.
        /// </summary>
        /// <param name="argument">The string that will be validated.</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <i>argument</i> is an empty string.</exception>
        public static void EnsureNotNullOrEmpty(string argument, string name)
        {
            EnsureNotNull(argument, name);
            if (string.IsNullOrEmpty(argument))
				throw new ArgumentException("The value cannot be an empty string.", name);
		}

        /// <summary>
        /// Used to verify that the provided string is not null and that it doesn't contain only white space.
        /// </summary>
        /// <param name="argument">The string that will be validated.</param>
        /// <param name="name">The name of the <i>argument</i> that will be used to identify it should an exception be thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>argument</i> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <i>argument</i> is either an empty string or contains only white space.</exception>
        public static void EnsureNotNullOrWhiteSpace(string argument, string name)
        {
            EnsureNotNull(argument, name);
            if (string.IsNullOrWhiteSpace(argument))
				throw new ArgumentException("The value cannot be an empty string or contain only whitespace.", name);
		}

        /// <summary>
        /// Used to verify that the provided argument is greater than the minimum value
        /// </summary>
        /// <param name="argument">argument to check</param>
        /// <param name="name">Name of the argument</param>
        /// <param name="minimum">Value argument must be greater than</param>
        /// <typeparam name="T">Type of the argument. Must be IComparable</typeparam>
        public static void EnsureGreaterThan<T>(T argument, string name, T minimum) where T : IComparable<T>
        {
            if (argument.CompareTo(minimum) <= 0)
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The provided value must be greater than {0}.", minimum), name);
		}
    }
}
