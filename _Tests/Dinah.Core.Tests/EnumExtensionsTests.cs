namespace EnumExtensionsTests
{
	public enum ByteEnum : byte
	{
		None,
		One,
		Two,
		[System.ComponentModel.Description("three val")]
		Three,
		Four
	}

	public enum IntEnum
	{
		None,
		One,
		Two,
		[System.ComponentModel.Description("three val")]
		Three,
		Four
	}

	[Flags]
	public enum FlagsEnum
	{
		[System.ComponentModel.Description("no val")]
		None = 0,
		One = 1,
		[System.ComponentModel.Description("two val")]
		Two = 2,
		[System.ComponentModel.Description("four val")]
		Four = 4,
		Eight = 8
	}

	[TestClass]
	public class ToValues
	{

		[TestMethod]
		public void byte_enum_throws()
			=> Assert.ThrowsException<InvalidCastException>(() => ByteEnum.Four.ToValues().ToArray());

		[TestMethod]
		public void non_flag_gives_inaccurate_results()
		{
			// Foo == decimal 3 == binary 11
			// binary 11 matches binary 01, 10, 11
			var three = IntEnum.Three;
			var threeValues = three.ToValues();
			ShouldBeEquivalentTo(threeValues, new[] { IntEnum.One, IntEnum.Two, IntEnum.Three });

			var _2or4 = IntEnum.Three | IntEnum.Four;
			var _2or4Values = _2or4.ToValues();
			ShouldBeEquivalentTo(_2or4Values, new[] { IntEnum.One, IntEnum.Two, IntEnum.Three, IntEnum.Four });
		}

		[TestMethod]
		public void enumerate_flags()
		{
			var flags = FlagsEnum.None | FlagsEnum.One | FlagsEnum.Four;
			var array = flags.ToValues().ToArray();

			var expectedArray = new[] { FlagsEnum.One, FlagsEnum.Four };
			array.ShouldBeEquivalentTo(expectedArray);
		}

		public static void ShouldBeEquivalentTo(IEnumerable<IntEnum> values, params IntEnum[] becauseArgs)
			=> values.ToArray().ShouldBeEquivalentTo(becauseArgs);
	}

	[TestClass]
	public class Include
	{
		[TestMethod]
		public void assign_single_value()
		{
			var options = FlagsEnum.None;
			options = options.Include(FlagsEnum.One);
			options.ShouldBeEquivalentTo(new[] { FlagsEnum.One });
		}

		[TestMethod]
		public void assign_multiple_values()
		{
			var options = FlagsEnum.One;
			options = options.Include(FlagsEnum.Two | FlagsEnum.Four);
			options.ShouldBeEquivalentTo(new[] { FlagsEnum.One, FlagsEnum.Two, FlagsEnum.Four });
		}
	}

	[TestClass]
	public class Remove
	{
		[TestMethod]
		public void remove_single_value()
		{
			var options = FlagsEnum.One | FlagsEnum.Two | FlagsEnum.Four;
			options = options.Remove(FlagsEnum.One);
			options.ShouldBeEquivalentTo(new[] { FlagsEnum.Two, FlagsEnum.Four });
		}

		[TestMethod]
		public void remove_multiple_values()
		{
			var options = FlagsEnum.One | FlagsEnum.Two | FlagsEnum.Four;
			options = options.Remove(FlagsEnum.One | FlagsEnum.Two);
			options.ShouldBeEquivalentTo(new[] { FlagsEnum.Four });
		}
	}

	// this one is actually built in. including here as a reminder
	[TestClass]
	public class HasFlag
	{
		[TestMethod]
		public void example()
		{
			var options = FlagsEnum.Two | FlagsEnum.Four;
			options.HasFlag(FlagsEnum.Two).ShouldBeTrue();
		}
	}

	[TestClass]
	public class MissingFlag
	{
		[TestMethod]
		public void example()
		{
			var options = FlagsEnum.Two | FlagsEnum.Four;
			options.MissingFlag(FlagsEnum.One).ShouldBeTrue();
		}
	}

	public static class AssertExt
	{
		public static void ShouldBeEquivalentTo(this FlagsEnum flagEnum, params FlagsEnum[] becauseArgs)
			=> flagEnum.ToValues().ToArray().ShouldBeEquivalentTo(becauseArgs);
	}
}
