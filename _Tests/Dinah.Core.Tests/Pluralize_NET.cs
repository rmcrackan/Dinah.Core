namespace PluralizerTests
{
	[TestClass]
	public class Pluralize_NET
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

		[TestMethod]
		public void Test_Pluralize_NET()
		{
			foreach (var kvp in Dictionary)
			{
				var sing = kvp.Key;
				var pl = kvp.Value;

				var p = new Pluralize.NET.Pluralizer();

				p.Singularize(sing).ShouldBe(sing);
				p.Singularize(pl).ShouldBe(sing);

				p.Pluralize(sing).ShouldBe(pl);
				p.Pluralize(pl).ShouldBe(pl);

				p.IsSingular(sing).ShouldBeTrue();
				p.IsPlural(pl).ShouldBeTrue();

				if (sing != pl)
				{
					p.IsSingular(pl).ShouldBeFalse();
					p.IsPlural(sing).ShouldBeFalse();
				}

				p.Format(sing, 0).ShouldBe(pl);
				p.Format(sing, 1).ShouldBe(sing);
				p.Format(sing, 5).ShouldBe(pl);
				p.Format(pl, 0).ShouldBe(pl);
				p.Format(pl, 1).ShouldBe(sing);
				p.Format(pl, 5).ShouldBe(pl);

				p.Format(sing, 0, true).ShouldBe("0 " + pl);
				p.Format(sing, 1, true).ShouldBe("1 " + sing);
				p.Format(sing, 5, true).ShouldBe("5 " + pl);
				p.Format(pl, 0, true).ShouldBe("0 " + pl);
				p.Format(pl, 1, true).ShouldBe("1 " + sing);
				p.Format(pl, 5, true).ShouldBe("5 " + pl);
			}
		}
	}
}
