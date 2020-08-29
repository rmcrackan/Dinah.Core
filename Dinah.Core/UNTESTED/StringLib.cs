using System;

namespace Dinah.Core
{
	public static class StringLib
	{
		public static string ToBase64(string text)
		{
			if (string.IsNullOrEmpty(text))
				return text;
			var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
			return Convert.ToBase64String(textBytes);
		}

		public static string FromBase64(string base64EncodedText)
		{
			if (string.IsNullOrEmpty(base64EncodedText))
				return base64EncodedText;
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedText);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
