using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Dinah.Core
{
	public static class HtmlHelper
	{
		public static Dictionary<string, string> GetInputs(string body)
		{
			// ONLY check for null body, not blank or whitespace
			ArgumentValidator.EnsureNotNull(body, nameof(body));

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
			=> GetElements(body, "a", "class", className)
			.Select(node => node.Attributes["href"]?.Value)
			.Where(href => !string.IsNullOrWhiteSpace(href))
			.ToList();

		public static int GetDivCount(string body, string id = null) => GetElements(body, "div", "id", id).Count;

		public static List<HtmlNode> GetElements(string body, string tag, string attribName = null, string attribValue = null)
		{
			// ONLY check for null body, not blank or whitespace
			ArgumentValidator.EnsureNotNull(body, nameof(body));

			ArgumentValidator.EnsureNotNullOrWhiteSpace(tag, nameof(tag));

			var doc = new HtmlDocument();
			doc.LoadHtml(body);

			var xpath = string.IsNullOrWhiteSpace(attribName) || string.IsNullOrWhiteSpace(attribValue)
				? $"//{tag}"
				: $"//{tag}[@{attribName}='{attribValue}']";

			var nodes = doc.DocumentNode.SelectNodes(xpath)?.Cast<HtmlNode>().ToList();
			return nodes ?? new List<HtmlNode>();
		}
	}
}
