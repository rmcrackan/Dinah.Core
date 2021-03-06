﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Dinah.Core
{
	public static class EnumExtensions
	{
		// https://stackoverflow.com/a/22132996
		public static IEnumerable<T> ToValues<T>(this T flags) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type.");

			var inputInt = (int)(object)flags;
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				var valueInt = (int)(object)value;
				if (valueInt >= 0 && 0 != (valueInt & inputInt))
					yield return value;
			}
		}

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
	}
}
