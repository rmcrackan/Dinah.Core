using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dinah.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace ArgumentValidatorTests
{
	[TestClass]
	public class EnsureNotNull
	{
		[TestMethod]
		public void null_fails() => Assert.ThrowsException<ArgumentNullException>(() => ArgumentValidator.EnsureNotNull((string)null, "name"));

		[TestMethod]
		public void blank_passes()
		{
			ArgumentValidator.EnsureNotNull("", "foo");
			ArgumentValidator.EnsureNotNull("   ", "foo");
		}

		[TestMethod]
		public void struct_passes()
		{
			ArgumentValidator.EnsureNotNull(DateTime.Now, "foo");
			ArgumentValidator.EnsureNotNull(0, "foo");
		}

		[TestMethod]
		public void object_passes()
		{
			ArgumentValidator.EnsureNotNull(new object(), "foo");
		}
	}

	[TestClass]
	public class EnsureEnumerableNotNullOrEmpty
	{
		[TestMethod]
		public void null_throws() => Assert.ThrowsException<ArgumentNullException>(() => ArgumentValidator.EnsureEnumerableNotNullOrEmpty((List<string>)null, "foo"));

		[TestMethod]
		public void empty_throws() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureEnumerableNotNullOrEmpty(new List<string>(), "foo"));

		[TestMethod]
		public void has_items_passes() => ArgumentValidator.EnsureEnumerableNotNullOrEmpty(new List<string> { null }, "foo");
	}

	[TestClass]
	public class EnsureNotNullOrEmpty
	{
		[TestMethod]
		public void null_throws() => Assert.ThrowsException<ArgumentNullException>(() => ArgumentValidator.EnsureNotNullOrEmpty(null, "foo"));

		[TestMethod]
		public void empty_throws() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureNotNullOrEmpty("", "foo"));

		[TestMethod]
		public void whitespace_passes() => ArgumentValidator.EnsureNotNullOrEmpty("   ", "foo");

		[TestMethod]
		public void has_value_passes() => ArgumentValidator.EnsureNotNullOrEmpty("bar", "foo");
	}

	[TestClass]
	public class EnsureNotNullOrWhiteSpace
	{
		[TestMethod]
		public void null_throws() => Assert.ThrowsException<ArgumentNullException>(() => ArgumentValidator.EnsureNotNullOrWhiteSpace(null, "foo"));

		[TestMethod]
		public void empty_throws() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureNotNullOrWhiteSpace("", "foo"));

		[TestMethod]
		public void whitespace_throws() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureNotNullOrWhiteSpace("   ", "foo"));

		[TestMethod]
		public void has_value_passes() => ArgumentValidator.EnsureNotNullOrWhiteSpace("bar", "foo");
	}

	[TestClass]
	public class EnsureGreaterThan
	{
		[TestMethod]
		public void null_argument_throws() => Assert.ThrowsException<NullReferenceException>(() => ArgumentValidator.EnsureGreaterThan(null, "n", "foo"));

		[TestMethod]
		public void too_small_throws() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureGreaterThan(9, "n", 10));

		[TestMethod]
		public void equals_fails() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureGreaterThan(9, "n", 9));

		[TestMethod]
		public void bigger_passes() => ArgumentValidator.EnsureGreaterThan(9, "n", 8);

		[TestMethod]
		public void null_minimum_passes() => ArgumentValidator.EnsureGreaterThan("arg", "n", null);
	}

	[TestClass]
	public class EnsureBetweenInclusive
	{
		[TestMethod]
		public void null_argument_throws() => Assert.ThrowsException<NullReferenceException>(() => ArgumentValidator.EnsureBetweenInclusive(null, "n", "min", "max"));

		[TestMethod]
		public void too_small_throws() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureBetweenInclusive(9, "n", 10, 20));

		[TestMethod]
		public void too_big_throws() => Assert.ThrowsException<ArgumentException>(() => ArgumentValidator.EnsureBetweenInclusive(21, "n", 10, 20));

		[TestMethod]
		public void minimum_passes() => ArgumentValidator.EnsureBetweenInclusive(10, "n", 10, 20);

		[TestMethod]
		public void between_passes() => ArgumentValidator.EnsureBetweenInclusive(15, "n", 10, 20);

		[TestMethod]
		public void maximum_passes() => ArgumentValidator.EnsureBetweenInclusive(20, "n", 10, 20);

		[TestMethod]
		public void null_minimum_passes() => ArgumentValidator.EnsureBetweenInclusive("arg", "n", null, "max");

		[TestMethod]
		public void null_maximum_passes() => ArgumentValidator.EnsureBetweenInclusive("arg", "n", "min", null);
	}
}
