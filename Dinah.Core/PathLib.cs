﻿using System;
using System.IO;

namespace Dinah.Core
{
	public static class PathLib
	{
		/// <summary>
		/// Use
		/// - one path with correct path and filename, not necessarily correct extension
		/// - another path with correct extension
		/// </summary>
		/// <param name="correctPathAndName">Absolute or relative file path or uri. Path and filename will be used. Extension may be changed.</param>
		/// <param name="correctExt">Absolute or relative file path or uri. Extension will be used</param>
		/// <returns></returns>
		public static string GetPathWithExtensionFromAnotherFile(
			string correctPathAndName,
			string correctExt)
		{
			if (correctPathAndName is null)
				throw new ArgumentNullException(nameof(correctPathAndName));
			if (string.IsNullOrWhiteSpace(correctPathAndName))
				throw new ArgumentException("Can not be null or white space", nameof(correctPathAndName));

			if (correctExt is null)
				throw new ArgumentNullException(nameof(correctExt));
			if (string.IsNullOrWhiteSpace(correctExt))
				throw new ArgumentException("Can not be null or white space", nameof(correctExt));


			if (Uri.TryCreate(correctExt, UriKind.Absolute, out var url))
				correctExt = url.AbsolutePath;

			if (!Path.HasExtension(correctExt))
				throw new FormatException($"{nameof(correctExt)} does not have a file extension: {correctExt}");

			var final = Path.ChangeExtension(correctPathAndName, Path.GetExtension(correctExt));
			return final;
		}

		private static string invalidChars { get; } = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
		public static string ToPathSafeString(string str, string replacement = "")
		{
			replacement ??= "";
			foreach (var ch in invalidChars)
				str = str.Replace(ch.ToString(), replacement);
			return str;
		}

		public static string ReplaceExtension(string filepath, string newExt)
		{
			var dir = Path.GetDirectoryName(filepath);
			var filenameNoExt = Path.GetFileNameWithoutExtension(filepath);

			newExt = newExt?.Trim().Trim('.').Trim() ?? "";
			if (newExt != "")
				newExt = "." + newExt;

			return Path.Combine(dir, filenameNoExt + newExt);
		}
	}
}
