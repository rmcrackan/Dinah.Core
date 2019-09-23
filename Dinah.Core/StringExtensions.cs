﻿using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Dinah.Core.Collections.Generic;

namespace Dinah.Core
{
	public static class StringExtensions
	{
		public static bool EqualsInsensitive(this string str1, string str2) => string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);

		public static bool StartsWithInsensitive(this string str1, string str2) => str1.StartsWith(str2, StringComparison.OrdinalIgnoreCase);

		public static bool EndsWithInsensitive(this string str1, string str2) => str1.EndsWith(str2, StringComparison.OrdinalIgnoreCase);

		public static bool ContainsInsensitive(this string str1, string str2) => str1?.IndexOf(str2, StringComparison.OrdinalIgnoreCase) >= 0;

		public static string Pluralize(this string str, int qty) => new Pluralize.NET.Pluralizer().Format(str, qty);

		/// <summary>return qty and noun</summary>
		public static string PluralizeWithCount(this string str, int qty) => new Pluralize.NET.Pluralizer().Format(str, qty, true);

		public static string FirstCharToUpper(this string str)
		{
			if (str == null)
				return null;
			if (string.IsNullOrWhiteSpace(str))
				return str;

			return char.ToUpper(str[0]) + str.Substring(1);
		}

		public static string Truncate(this string str, int limit)
			=> str is null ? null
			: str.Length.In(0, 1) ? str
			: limit < 1 ? str.Substring(0, 1)
			: str.Length >= limit ? str.Substring(0, limit)
			: str;

		public static string SurroundWithQuotes(this string str) => "\"" + str + "\"";

		public static string ExtractString(this string haystack, string before, int needleLength)
		{
			if (string.IsNullOrWhiteSpace(haystack))
				return null;

			if (string.IsNullOrWhiteSpace(before))
				return null;

			ArgumentValidator.EnsureGreaterThan(needleLength, nameof(needleLength), 0);

			var index = haystack.IndexOf(before);

			if (index < 0)
				return null;

			// needed to avoid ArgumentOutOfRangeException for too-long needleLength
			var min = Math.Min(index + before.Length, needleLength);

			var needle = haystack.Substring(index + before.Length, min);

			return needle;
		}

		/// <summary>A very forgiving interpretation of a string to a boolean.</summary>
		public static bool ToBoolean(this string str)
			=> str is null ? false : str.Trim().ToLower(CultureInfo.InvariantCulture).In("y", "1", "t", "true");

		private const string _validHexChars = "0123456789ABCDEFabcdef";
		/// <summary>
		/// Extension for string to interpret as hexadecimal string and convert to a byte array
		/// </summary>
		/// <param name="hexValue">The hexadecimal string (such as "0001AF3cab") to convert into a byte array.</param>
		/// <returns>A byte array containing the bytes represented by the supplied string.</returns>
		/// <exception cref="ArgumentException">
		///   If the string contains non-hexadecimal characters (except for leading or trailing white space, which is ignored), or if it 
		///   does not contain an even number of characters (since 2 characters are needed to represent each byte).
		/// </exception>
		public static byte[] HexStringToByteArray(this string hexValue)
		{
			hexValue = hexValue?.Trim();

			ArgumentValidator.EnsureNotNullOrEmpty(hexValue, nameof(hexValue));

			var hasInvalidChars = hexValue.Except(_validHexChars).Any();
			if (hasInvalidChars)
				throw new ArgumentException("string contains invalid hexadecimal characters.", nameof(hexValue));

			if ((hexValue.Length % 2) != 0)
				throw new ArgumentException("string does not contain an even number of hexadecimal characters.", nameof(hexValue));

			var result = new byte[hexValue.Length / 2];

			for (var index = 0; index < hexValue.Length; index += 2)
				result[index / 2] = byte.Parse(hexValue.Substring(index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

			return result;
		}

		public static string ToMd5Hash(this string str)
		{
			ArgumentValidator.EnsureNotNull(str, nameof(str));

			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				var hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));

				var sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
					sb.Append(hashBytes[i].ToString("X2"));
				return sb.ToString();
			}
		}
	}
}
