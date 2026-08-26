namespace PluralizerTests
{
	/// <summary>
	/// Rule coverage for the internal pluralizer that replaced the Pluralize.NET package,
	/// exercised through the public extension methods. The irregular and uninflected words
	/// are covered by the Pluralize/PluralizeWithCount classes in StringExtensionsTests;
	/// this class pins the suffix rules and casing behavior.
	/// </summary>
	[TestClass]
	public class EnglishPluralizerTests
	{
		public static Dictionary<string, string> RuleWords { get; } = new()
		{
			// plain +s
			["book"] = "books",
			// vowel + y just takes s
			["day"] = "days",
			// consonant + y -> ies
			["city"] = "cities",
			// sibilant endings take es
			["glass"] = "glasses",
			["church"] = "churches",
			["dish"] = "dishes",
			["buzz"] = "buzzes",
			// in the irregulars table: "buses" cannot be told apart from "houses" by suffix alone
			["bus"] = "buses",
			["house"] = "houses",
		};

		[TestMethod]
		public void quantities_other_than_1_use_the_plural()
		{
			foreach (var (sing, pl) in RuleWords)
			{
				sing.Pluralize(0).ShouldBe(pl);
				sing.Pluralize(5).ShouldBe(pl);
			}
		}

		[TestMethod]
		public void quantity_1_uses_the_singular()
		{
			foreach (var (sing, _) in RuleWords)
				sing.Pluralize(1).ShouldBe(sing);
		}

		[TestMethod]
		public void an_already_plural_word_is_left_alone()
		{
			foreach (var (_, pl) in RuleWords)
			{
				pl.Pluralize(0).ShouldBe(pl);
				pl.Pluralize(5).ShouldBe(pl);
			}
		}

		[TestMethod]
		public void an_already_plural_word_is_singularized_for_quantity_1()
		{
			foreach (var (sing, pl) in RuleWords)
				pl.Pluralize(1).ShouldBe(sing);
		}

		[TestMethod]
		public void with_count_prepends_the_quantity()
		{
			"book".PluralizeWithCount(0).ShouldBe("0 books");
			"book".PluralizeWithCount(1).ShouldBe("1 book");
			"books".PluralizeWithCount(1).ShouldBe("1 book");
			"book".PluralizeWithCount(5).ShouldBe("5 books");
		}

		[TestMethod]
		public void casing_carries_over_to_suffixes_and_table_lookups()
		{
			"BOOK".Pluralize(2).ShouldBe("BOOKS");
			"CITY".Pluralize(2).ShouldBe("CITIES");
			"GLASS".Pluralize(2).ShouldBe("GLASSES");
			"Person".Pluralize(2).ShouldBe("People");
			"PERSON".Pluralize(2).ShouldBe("PEOPLE");
			"PEOPLE".Pluralize(1).ShouldBe("PERSON");
			"Mice".Pluralize(1).ShouldBe("Mouse");
		}

		[TestMethod]
		public void singular_lookalikes_are_not_mangled()
		{
			// end in s but are singular: the generic strip-the-s rule must not touch them
			"glass".Pluralize(1).ShouldBe("glass");
			"status".Pluralize(1).ShouldBe("status");
			"analysis".Pluralize(1).ShouldBe("analysis");
			"bus".Pluralize(1).ShouldBe("bus");
		}
	}
}
