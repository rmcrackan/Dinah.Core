using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestCommon;

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
				.Should().Be(@"C:\User\whatever\path\file.txt");
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
			var pattern = @"^\D*(?<index>\d+\.?\d*)";
			var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture);

			var match = regex.Match(input);
			var mStr = match.Groups["index"].ToString();
			
			var mFloat = float.Parse(mStr);
			mFloat.Should().Be(expected);
		}
	}
}
