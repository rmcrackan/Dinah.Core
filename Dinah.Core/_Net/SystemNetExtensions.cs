using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Reflection;

#nullable enable
namespace Dinah.Core.Net
{
	public static class SystemNetExtensions
	{
		public static IEnumerable<Cookie> EnumerateCookies(this CookieContainer cookieJar, Uri uri) => cookieJar.GetCookies(uri).Cast<Cookie>();

		public static string? Debug_GetCookies(this CookieContainer cookieJar, Uri uri)
			=> cookieJar
			.EnumerateCookies(uri)
			?.ToList()
			.Select(c => $"{c.Name}={c.Value}")
			.Aggregate("", (a, b) => $"{a};{b}")
			.Trim(';');

		// https://stackoverflow.com/a/14074200
		public static Hashtable? ReflectOverAllCookies(this CookieContainer cookies)
			=> cookies.GetType().InvokeMember(
				"m_domainTable",
				BindingFlags.NonPublic |
				BindingFlags.GetField |
				BindingFlags.Instance,
				null,
				cookies,
				new object[] { }) as Hashtable;
	}
}
