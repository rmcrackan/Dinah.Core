using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Dinah.Core
{
	public static class ObjectExtensions
	{
		public static string GetDescription<T>(this T e)
		{
			// identical
			//en.GetType().IsDefined(typeof(FlagsAttribute), false)
			//en.GetType().GetCustomAttributes<FlagsAttribute>().Any()
			//en.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0

			if (e is Enum en &&
				Convert.ToInt64(en) != 0 &&
				en.GetType().IsDefined(typeof(FlagsAttribute), false))
			{
				return en.getFlagDescriptions();
			}

			// non-enums, non-flag enums, and flag enums == 0
			return e.getDescription();
		}

		private static string getFlagDescriptions(this Enum en)
			=> Enum.GetValues(en.GetType()).Cast<Enum>()
				.Where(enumValue => en.HasFlag(enumValue) && Convert.ToInt64(enumValue) > 0)
				.Select(enumValue => enumValue.getDescription())
				.Aggregate((a, b) => $"{a ?? "[null]"} | {b ?? "[null]"}");

		private static string getDescription<T>(this T e)
		{
			var attribute =
				e.GetType()
				.GetTypeInfo()
				.GetMember(e.ToString())
				.FirstOrDefault(member => member.MemberType == MemberTypes.Field)
				?.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.SingleOrDefault()
				as DescriptionAttribute;

			return attribute?.Description;
		}

        public static bool In<T>(this T source, params T[] parameters) => _in(source, parameters);
        public static bool In<T>(this T source, IEnumerable<T> parameters) => _in(source, parameters);
        private static bool _in<T>(T source, IEnumerable<T> parameters) => parameters.Contains(source);
    }
}
