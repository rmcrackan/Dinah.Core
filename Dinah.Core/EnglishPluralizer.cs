using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Dinah.Core
{
	/// <summary>
	/// Small English pluralizer backing <see cref="StringExtensions.Pluralize"/> and
	/// <see cref="StringExtensions.PluralizeWithCount"/>. It replaces the retired Pluralize.NET
	/// dependency and keeps its observable behavior for the words this library's consumers use:
	/// regular nouns, the irregular and uninflected words listed below, casing carried over from
	/// the input, and inputs that are already in the requested form passing through unchanged.
	/// It is intentionally not a general-purpose inflector; add to the tables when a new word
	/// falls outside the rules.
	/// </summary>
	internal static class EnglishPluralizer
	{
		/// <summary>lowercase singular -> lowercase plural</summary>
		private static readonly Dictionary<string, string> irregulars = new()
		{
			["person"] = "people",
			["man"] = "men",
			["woman"] = "women",
			["child"] = "children",
			["foot"] = "feet",
			["tooth"] = "teeth",
			["goose"] = "geese",
			["mouse"] = "mice",
			["louse"] = "lice",
			["die"] = "dice",
			["index"] = "indices",
			["octopus"] = "octopi",
			// regular in speech but outside the suffix rules below
			["bus"] = "buses",
		};

		private static readonly Dictionary<string, string> irregularSingulars
			= irregulars.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

		private static readonly HashSet<string> uninflected = new()
		{ "fish", "deer", "sheep", "moose", "series", "species", "aircraft", "bison", "swine" };

		/// <summary>qty of 1 returns the singular form; any other qty (including 0) the plural.</summary>
		public static string Format(string word, int qty)
			=> qty == 1 ? Singularize(word) : Pluralize(word);

		public static string Pluralize(string word)
		{
			if (word.Length == 0)
				return word;

			var lower = word.ToLowerInvariant();

			if (uninflected.Contains(lower))
				return word;
			if (irregulars.TryGetValue(lower, out var irregularPlural))
				return matchCase(irregularPlural, word);
			if (irregularSingulars.ContainsKey(lower))
				return word;

			// already a rule-formed plural ("toes", "Boxes", "Entities"): leave it alone
			var singular = Singularize(word);
			if (!singular.Equals(word, StringComparison.Ordinal) && applyPluralRules(singular).Equals(word, StringComparison.Ordinal))
				return word;

			return applyPluralRules(word);
		}

		public static string Singularize(string word)
		{
			if (word.Length == 0)
				return word;

			var lower = word.ToLowerInvariant();

			if (uninflected.Contains(lower))
				return word;
			if (irregularSingulars.TryGetValue(lower, out var irregularSingular))
				return matchCase(irregularSingular, word);
			if (irregulars.ContainsKey(lower))
				return word;

			// entities -> entity
			if (word.Length > 3 && lower.EndsWith("ies"))
				return word[..^3] + suffixCased("y", word);

			// boxes -> box, buzzes -> buzz, churches -> church, dishes -> dish, glasses -> glass
			if (word.Length > 2 && lower.EndsWith("es"))
			{
				var stem = lower[..^2];
				if (stem.EndsWith("x") || stem.EndsWith("z") || stem.EndsWith("ch") || stem.EndsWith("sh") || stem.EndsWith("ss"))
					return word[..^2];
			}

			// houses -> house, toes -> toe, books -> book;
			// but glass, status, analysis are already singular
			if (word.Length > 1 && lower.EndsWith("s")
				&& !lower.EndsWith("ss") && !lower.EndsWith("us") && !lower.EndsWith("is"))
				return word[..^1];

			return word;
		}

		private static string applyPluralRules(string word)
		{
			var lower = word.ToLowerInvariant();

			// city -> cities, but day -> days
			if (word.Length > 1 && lower.EndsWith("y") && !isVowel(lower[^2]))
				return word[..^1] + suffixCased("ies", word);

			// glass -> glasses, box -> boxes, buzz -> buzzes, church -> churches, dish -> dishes
			if (lower.EndsWith("s") || lower.EndsWith("x") || lower.EndsWith("z") || lower.EndsWith("ch") || lower.EndsWith("sh"))
				return word + suffixCased("es", word);

			return word + suffixCased("s", word);
		}

		private static bool isVowel(char c) => c is 'a' or 'e' or 'i' or 'o' or 'u';

		private static bool isAllUpper(string word)
			=> word.Any(char.IsLetter) && !word.Any(char.IsLower);

		private static string suffixCased(string suffix, string word)
			=> isAllUpper(word) ? suffix.ToUpperInvariant() : suffix;

		/// <summary>Carry the input's casing (all-caps or leading capital) over to a table-lookup result.</summary>
		private static string matchCase(string result, string template)
		{
			if (isAllUpper(template))
				return result.ToUpperInvariant();
			if (char.IsUpper(template[0]))
				return char.ToUpperInvariant(result[0]) + result[1..];
			return result;
		}
	}
}
