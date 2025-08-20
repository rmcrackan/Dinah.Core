namespace StringExtensionsTests
{
    [TestClass]
    public class EqualsInsensitive
    {
        [TestMethod]
        [DataRow(null, "")]
        [DataRow(null, "   ")]
        [DataRow("", "   ")]
        [DataRow("   ", "      ")]
        [DataRow("<>", "  <>  ")]
        public void should_fail(string str1, string str2) => Assert.IsFalse(str1.EqualsInsensitive(str2));

        [TestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("   ", "   ")]
        [DataRow("hello world", "hello world")]
        [DataRow("hello world", "HELLO WORLD")]
        [DataRow("hello world", "Hello World")]
        [DataRow("hello world", "hELLo woRLd")]
        [DataRow("hello world<>!@#$%^&*()\"", "hELLo woRLd<>!@#$%^&*()\"")]
        public void should_pass(string str1, string str2) => Assert.IsTrue(str1.EqualsInsensitive(str2));
    }

    [TestClass]
    public class StartsWithInsensitive
    {
        [TestMethod]
        [DataRow(null, null)]
        [DataRow(null, "")]
        [DataRow(null, "   ")]
        public void should_throw_NullReferenceException(string fullString, string prefix)
            => Assert.ThrowsException<NullReferenceException>(() => fullString.StartsWithInsensitive(prefix));

        [TestMethod]
        [DataRow("", null)]
        [DataRow("   ", null)]
        public void should_throw_ArgumentNullException(string fullString, string prefix)
            => Assert.ThrowsException<ArgumentNullException>(() => fullString.StartsWithInsensitive(prefix));

        [TestMethod]
        [DataRow("", "   ")]
        [DataRow("  <>", "<>")]
        [DataRow("<>___", "<>  ")]
        [DataRow("<>___", "  <>")]
        public void should_fail(string fullString, string prefix) => Assert.IsFalse(fullString.StartsWithInsensitive(prefix));

        [TestMethod]
        [DataRow("   empty", "")]
        [DataRow("   3 spaces", "   ")]
        [DataRow("<>___", "<>")]
        [DataRow("hello world___", "hello world")]
        [DataRow("hello world___", "HELLO WORLD")]
        [DataRow("hello world___", "Hello World")]
        [DataRow("hello world___", "hELLo woRLd")]
        [DataRow("hello world<>!@#$%^&*()\"___", "hELLo woRLd<>!@#$%^&*()\"")]
        public void should_pass(string fullString, string prefix) => Assert.IsTrue(fullString.StartsWithInsensitive(prefix));
    }

    [TestClass]
    public class EndsWithInsensitive
    {
        [TestMethod]
        [DataRow(null, null)]
        [DataRow(null, "")]
        [DataRow(null, "   ")]
        public void should_throw_NullReferenceException(string fullString, string prefix)
            => Assert.ThrowsException<NullReferenceException>(() => fullString.EndsWithInsensitive(prefix));

        [TestMethod]
        [DataRow("", null)]
        [DataRow("   ", null)]
        public void should_throw_ArgumentNullException(string fullString, string prefix)
            => Assert.ThrowsException<ArgumentNullException>(() => fullString.EndsWithInsensitive(prefix));

        [TestMethod]
        [DataRow("", "   ")]
        [DataRow("<>  ", "<>")]
        [DataRow("___<>", "<>  ")]
        [DataRow("___<>", "  <>")]
        public void should_fail(string fullString, string suffix) => Assert.IsFalse(fullString.EndsWithInsensitive(suffix));

        [TestMethod]
        [DataRow("empty   ", "")]
        [DataRow("3 spaces   ", "   ")]
        [DataRow("___<>", "<>")]
        [DataRow("___hello world", "hello world")]
        [DataRow("___hello world", "HELLO WORLD")]
        [DataRow("___hello world", "Hello World")]
        [DataRow("___hello world", "hELLo woRLd")]
        [DataRow("___hello world<>!@#$%^&*()\"", "hELLo woRLd<>!@#$%^&*()\"")]
        public void should_pass(string fullString, string suffix) => Assert.IsTrue(fullString.EndsWithInsensitive(suffix));
    }

    [TestClass]
    public class ContainsInsensitive
    {
        [TestMethod]
        [DataRow("", null)]
        [DataRow("   ", null)]
        public void should_throw_ArgumentNullException(string fullString, string needle)
            => Assert.ThrowsException<ArgumentNullException>(() => fullString.ContainsInsensitive(needle));

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(null, "")]
        [DataRow(null, "   ")]
        [DataRow("", "   ")]
        [DataRow("<>", "  <>  ")]
        [DataRow("  <>", "  <>  ")]
        [DataRow("<>  ", "  <>  ")]
        public void should_fail(string fullString, string needle) => Assert.IsFalse(fullString.ContainsInsensitive(needle));

        [TestMethod]
        [DataRow("empty", "")]
        [DataRow("3   spaces", "")]
        [DataRow("___<>___", "<>")]
        [DataRow("___hello world___", "hello world")]
        [DataRow("___hello world___", "HELLO WORLD")]
        [DataRow("___hello world___", "Hello World")]
        [DataRow("___hello world___", "hELLo woRLd")]
        [DataRow("___hello world<>!@#$%^&*()\"___", "hELLo woRLd<>!@#$%^&*()\"")]
        public void should_pass(string fullString, string needle) => Assert.IsTrue(fullString.ContainsInsensitive(needle));
    }

	public class pluralize_shared
	{
		public static Dictionary<string, string> Dictionary { get; } = new Dictionary<string, string>
		{
			["toe"] = "toes",
			["shoe"] = "shoes",
			["Entity"] = "Entities",
			["PERSON"] = "PEOPLE",
			["fish"] = "fish",
			["deer"] = "deer",
			["sheep"] = "sheep",
			["house"] = "houses",
			["mouse"] = "mice",
			["louse"] = "lice",
			["Box"] = "Boxes",
			["index"] = "indices",
			["OCTOPUS"] = "OCTOPI",
			["man"] = "men",
			["woman"] = "women"
		};
	}

	[TestClass]
	public class Pluralize
	{
		[TestMethod]
		public void _0_times_uses_plural()
		{
			foreach (var kvp in pluralize_shared.Dictionary)
			{
				var sing = kvp.Key;
				var pl = kvp.Value;

				sing.Pluralize(0).ShouldBe(pl);
				pl.Pluralize(0).ShouldBe(pl);
			}
		}

		[TestMethod]
		public void _1_times_uses_singlular()
		{
			foreach (var kvp in pluralize_shared.Dictionary)
			{
				var sing = kvp.Key;
				var pl = kvp.Value;

				sing.Pluralize(1).ShouldBe(sing);
				pl.Pluralize(1).ShouldBe(sing);
			}
		}

		[TestMethod]
		public void many_times_uses_plural()
		{
			foreach (var kvp in pluralize_shared.Dictionary)
			{
				var sing = kvp.Key;
				var pl = kvp.Value;

				sing.Pluralize(5).ShouldBe(pl);
				pl.Pluralize(5).ShouldBe(pl);
			}
		}
	}

	[TestClass]
	public class PluralizeWithCount
	{
		[TestMethod]
		public void _0_times_uses_plural()
		{
			foreach (var kvp in pluralize_shared.Dictionary)
			{
				var sing = kvp.Key;
				var pl = kvp.Value;

				sing.PluralizeWithCount(0).ShouldBe("0 " + pl);
				pl.PluralizeWithCount(0).ShouldBe("0 " + pl);
			}
		}

		[TestMethod]
		public void _1_times_uses_singlular()
		{
			foreach (var kvp in pluralize_shared.Dictionary)
			{
				var sing = kvp.Key;
				var pl = kvp.Value;

				sing.PluralizeWithCount(1).ShouldBe("1 " + sing);
				pl.PluralizeWithCount(1).ShouldBe("1 " + sing);
			}
		}

		[TestMethod]
		public void many_times_uses_plural()
		{
			foreach (var kvp in pluralize_shared.Dictionary)
			{
				var sing = kvp.Key;
				var pl = kvp.Value;

				sing.PluralizeWithCount(5).ShouldBe("5 " + pl);
				pl.PluralizeWithCount(5).ShouldBe("5 " + pl);
			}
		}
	}

	[TestClass]
	public class FirstCharToUpper
	{
		[TestMethod]
		[DataRow(null, null)]
		[DataRow("", "")]
		[DataRow("   ", "   ")]
		[DataRow("foo", "Foo")]
		[DataRow("Foo", "Foo")]
		[DataRow("FOO", "FOO")]
		[DataRow("foo bar", "Foo bar")]
		public void test_outputs(string param, string expected)
			=> param.FirstCharToUpper().ShouldBe(expected);
	}

	[TestClass]
	public class Truncate
	{
		[TestMethod]
		public void null_returns_null()
			=> ((string)null).Truncate(10).ShouldBe(null);

		[TestMethod]
		public void empty_returns_empty()
			=> "".Truncate(10).ShouldBe("");

		[TestMethod]
		public void single_char_string_returns_char()
			=> "F".Truncate(10).ShouldBe("F");

		[TestMethod]
		public void _0_limit_returns_char()
			=> "foo".Truncate(0).ShouldBe("f");

		[TestMethod]
		public void negative_limit_returns_char()
			=> "foo".Truncate(-1).ShouldBe("f");

		[TestMethod]
		public void string_shorter_than_limit_is_truncated()
			=> "foo".Truncate(2).ShouldBe("fo");

		[TestMethod]
		public void string_same_length_as_limit_is_returned()
			=> "foo".Truncate(3).ShouldBe("foo");

		[TestMethod]
		public void string_longer_than_limit_is_returned()
			=> "foo".Truncate(4).ShouldBe("foo");
	}

	[TestClass]
	public class SurroundWithQuotes
	{
		[TestMethod]
		[DataRow(null, "\"\"")]
		[DataRow("", "\"\"")]
		[DataRow("   ", "\"   \"")]
		[DataRow("foo", "\"foo\"")]
		[DataRow("foo bar", "\"foo bar\"")]
		public void test_outputs(string param, string expected)
			=> param.SurroundWithQuotes().ShouldBe(expected);
	}

	[TestClass]
	public class ExtractString
	{
		[TestMethod]
		public void null_haystack_returns_null()
			=> ((string)null).ExtractString("", 1).ShouldBeNull();

		[TestMethod]
		public void null_before_returns_null()
			=> "foo".ExtractString(null, 1).ShouldBeNull();

		[TestMethod]
		public void empty_before_returns_null()
			=> "foo".ExtractString("", 1).ShouldBeNull();

		[TestMethod]
		public void whitespace_before_returns_null()
			=> "foo".ExtractString("   ", 1).ShouldBeNull();

		[TestMethod]
		public void _0_needleLength_throws()
			=> Assert.ThrowsException<ArgumentException>(() => "foo".ExtractString("f", 0));

		[TestMethod]
		public void negative_needleLength_throws()
			=> Assert.ThrowsException<ArgumentException>(() => "foo".ExtractString("f", -1));

		[TestMethod]
		public void needle_not_found_returns_null()
			=> "foo".ExtractString("bar", 1).ShouldBeNull();

		[TestMethod]
		[DataRow("foobar", "foo", 1, "b")]
		[DataRow("foobar", "foo", 3, "bar")]
		public void needle_found(string haystack, string before, int needleLength, string expected)
			=> haystack.ExtractString(before, needleLength).ShouldBe(expected);

		[TestMethod]
		public void needleLength_too_big_returns_existing()
			=> "foobar".ExtractString("foo", 4).ShouldBe("bar");
	}

	[TestClass]
	public class ToBoolean
	{
		[TestMethod]
		[DataRow("y")]
		[DataRow("   y   ")]
		[DataRow("Y")]
		[DataRow("1")]
		[DataRow("   1   ")]
		[DataRow("t")]
		[DataRow("   t   ")]
		[DataRow("T")]
		[DataRow("true")]
		[DataRow("   true   ")]
		[DataRow("True")]
		[DataRow("TRUE")]
		public void is_true(string str)
			=> str.ToBoolean().ShouldBeTrue();

		[TestMethod]
		[DataRow(null)]
		[DataRow("")]
		[DataRow("   ")]
		[DataRow("n")]
		[DataRow("N")]
		[DataRow("f")]
		[DataRow("false")]
		[DataRow("Foo")]
		public void is_false(string str)
			=> str.ToBoolean().ShouldBeFalse();
	}

	[TestClass]
	public class HexStringToByteArray
	{
		[TestMethod]
		public void null_throws()
			=> Assert.ThrowsException<ArgumentNullException>(() => ((string)null).HexStringToByteArray());

		[TestMethod]
		public void empty_or_whitespace_throws()
		{
			Assert.ThrowsException<ArgumentException>(() => "".HexStringToByteArray());
			Assert.ThrowsException<ArgumentException>(() => "   ".HexStringToByteArray());
		}

		[TestMethod]
		public void invalid_char_throws()
		{
			Assert.ThrowsException<ArgumentException>(() => "1z".HexStringToByteArray());
			Assert.ThrowsException<ArgumentException>(() => "z".HexStringToByteArray());
		}

		[TestMethod]
		public void uneven_char_count_throws()
		{
			Assert.ThrowsException<ArgumentException>(() => "1".HexStringToByteArray());
			Assert.ThrowsException<ArgumentException>(() => "abc".HexStringToByteArray());
		}

		[TestMethod]
		[DataRow("00", new byte[] { 0 })]
		[DataRow("0102", new byte[] { 1, 2 })]
		[DataRow("0a0b0c0d", new byte[] { 10, 11, 12, 13 })]
		public void test_outputs(string str, byte[] bytes)
			=> str.HexStringToByteArray().ShouldBeEquivalentTo(bytes);
	}

	[TestClass]
	public class ToMd5Hash
	{
		[TestMethod]
		public void null_throws()
			=> Assert.ThrowsException<ArgumentNullException>(() => ((string)null).ToMd5Hash());

		[TestMethod]
		[DataRow("", "D41D8CD98F00B204E9800998ECF8427E")]
		[DataRow("foo", "ACBD18DB4CC2F85CEDEF654FCCC4A4D8")]
		[DataRow("test me", "7B0C2C2CBC980155D71BA3BE4D174F56")]
		public void test_outputs(string input, string expected)
			=> input.ToMd5Hash().ShouldBe(expected);
	}

	[TestClass]
	public class ToMask
	{
		[TestMethod]
		// null, blank, empty, whitespace
		[DataRow(null, null)]
		[DataRow("", "")]
		[DataRow("   ", "   ")]
		[DataRow(" \r\n a\r\nb\r\nc \r\n ", " \r\n a[...]c \r\n ")]
		// basic strings
		[DataRow("a", "[...]")]
		[DataRow("ab", "a[...]b")]
		[DataRow("  ab", "  a[...]b")]
		[DataRow("ab  ", "a[...]b  ")]
		[DataRow("  ab  ", "  a[...]b  ")]
		[DataRow("   a   ", "   [...]   ")]
		[DataRow("password", "p[...]d")]
		// segment delimiters only
		[DataRow("//:@", "//:@")]
		[DataRow("  //:@  ", "  //:@  ")]
		// email
		[DataRow("test@me.com", "t[...]t@m[...]e.c[...]m")]
		[DataRow("test@me.co.uk", "t[...]t@m[...]e.c[...]o.u[...]k")]
		[DataRow("a@b.co", "[...]@[...].c[...]o")]
		[DataRow("@b.co", "@[...].c[...]o")]
		// ipv4
		[DataRow("192.168.1.1", "1[...]2.1[...]8.[...].[...]")]
		// ipv6
		[DataRow("::/0", "::/[...]")]
		[DataRow("::/128", "::/1[...]8")]
		[DataRow("2001:1::2/128", "2[...]1:[...]::[...]/1[...]8")]
		// url
		[DataRow("https://test.co.uk", "h[...]s://t[...]t.c[...]o.u[...]k")]
		// file path
		[DataRow(@"C:\my\file\here.txt", @"[...]:\m[...]y\f[...]e\h[...]e.t[...]t")]
		public void test_outputs(string input, string expected)
			=> StringExtensions.ToMask(input, "[...]").ShouldBe(expected);
	}
}
