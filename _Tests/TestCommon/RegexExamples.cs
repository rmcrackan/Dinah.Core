using System.Text.RegularExpressions;

namespace RegexExamples
{
	[TestClass]
	public class Replace
	{
		[TestMethod]
		public void case_insensitive_replace()
		{
			var input = @"%DeskToP%\path\file.txt";
			// these characters are literals, not patterns
			var pattern = Regex.Escape("%desktop%");
			var replacement = @"C:\User\whatever";

			Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase)
				.ShouldBe(@"C:\User\whatever\path\file.txt");
		}
	}

	[TestClass]
	public class Match
	{
		[TestMethod]
		[DataRow("abc1.12.def3.4", 1.12f)]
		[DataRow("0.3,4.7,8.6", 0.3f)]
		[DataRow("0.6, 3.5", 0.6f)]
		[DataRow("5-8", 5f)]
		[DataRow("5.", 5f)]
		[DataRow("X.5", 5f)] // no leading 0
		public void get_first_number(string input, float expected)
		{
			var regex = new Regex(@"^\D*(?<index>\d+\.?\d*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

			var match = regex.Match(input);
			var mStr = match.Groups["index"].ToString();
			
			var mFloat = float.Parse(mStr);
			mFloat.ShouldBe(expected);
		}
	}
}
