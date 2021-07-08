using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Dinah.Core
{
	public static class HtmlHelper
	{
		public static Dictionary<string, string> GetInputs(string body)
		{
			body = body ?? throw new ArgumentNullException(nameof(body));

			var inputs = new Dictionary<string, string>();

			var doc = new HtmlDocument();
			doc.LoadHtml(body);

			var nodes = doc.DocumentNode.SelectNodes(".//input");

			if (nodes is null)
				return inputs;

			foreach (var node in nodes)
			{
				var name = node.Attributes["name"]?.Value;
				var value = node.Attributes["value"]?.Value;

				if (!string.IsNullOrWhiteSpace(name))
					inputs[name] = value;
			}

			return inputs;
		}

		public static List<string> GetLinks(string body, string className = null)
		{
			body = body ?? throw new ArgumentNullException(nameof(body));

			var links = new List<string>();

			var doc = new HtmlDocument();
			doc.LoadHtml(body);

			var xpath
				= string.IsNullOrWhiteSpace(className)
				? $"//a"
				: $"//a[@class='{className?.Trim()}']";

			var nodes = doc.DocumentNode.SelectNodes(xpath);

			if (nodes is null)
				return links;

			foreach (HtmlNode node in nodes)
			{
				var href = node.Attributes["href"]?.Value;

				if (!string.IsNullOrWhiteSpace(href))
					links.Add(href);
			}

			return links;
		}

		public static int GetDivCount(string body, string id = null)
		{
			body = body ?? throw new ArgumentNullException(nameof(body));

			var divs = new List<string>();

			var doc = new HtmlDocument();
			doc.LoadHtml(body);

			var xpath = string.IsNullOrWhiteSpace(id)
				? $"//div"
				: $"//div[@id='{id?.Trim()}']";

			var nodes = doc.DocumentNode.SelectNodes(xpath);

			return nodes?.Count ?? 0;
		}
	}
}
