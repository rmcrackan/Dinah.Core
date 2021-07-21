using System;
using System.Collections.Generic;
using System.Linq;

namespace Dinah.Core
{
    //
    // These do the same thing with different syntax
    // Enum<MyEnum>.GetValues()
    // MyEnum.MyFirst.GetValues<MyEnum>()
    // EnumExtensions.GetValues<MyEnum>() 
    //

    #region class method/s
    /// <summary>Enum Extension Methods</summary>
    /// <typeparam name="T">type of Enum</typeparam>
    public static class Enum<T> where T : Enum
    {
        public static T Parse(string value) => (T)Enum.Parse(typeof(T), value);

        public static IEnumerable<T> GetValues() => Enum.GetValues(typeof(T)).Cast<T>();

        public static int ToInt(T obj) => Convert.ToInt32(Enum.Parse(typeof(T), obj.ToString()) as Enum);

        /// <summary>get count of enum options except "None"</summary>
        public static int Count => GetValues().Count(obj => ToInt(obj) > 0);
    }
    #endregion

    public static class EnumExtensions
    {
        /// <summary>
        /// Gets all items for this enum type. Value param itself does nothing except provide the correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">One of the values of this enum type.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(this Enum value) => Enum.GetValues(typeof(T)).Cast<T>();

        /// <summary>
        /// Gets all items for an enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>() where T : struct => Enum.GetValues(typeof(T)).Cast<T>();


        #region object/instance methods. flags
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

        // most from: http://hugoware.net/blog/enumeration-extensions-2-0
        public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum input, bool checkZero = false, bool checkFlags = true)
            where TEnum : Enum
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                yield break;

            ulong setBits = Convert.ToUInt64(input);
            // if no flags are set, return empty
            if (!checkZero && (0 == setBits))
                yield break;

            // if it's not a flag enum, return empty
            if (checkFlags && !input.GetType().IsDefined(typeof(FlagsAttribute), false))
                yield break;

            // check each enum value mask if it is in input bits
            foreach (TEnum value in Enum<TEnum>.GetValues())
            {
                ulong valMask = Convert.ToUInt64(value);

                if ((setBits & valMask) == valMask)
                    yield return value;
            }
        }

        public static T IncludeOrRemove<T>(this Enum value, T flag, bool addRemoveFlag)
        {
            if (addRemoveFlag)
                return value.Include(flag);
            else
                return value.Remove(flag);
        }

        /// <summary>Includes an enumerated type and returns the new value</summary>
        public static T Include<T>(this Enum value, T append)
        {
            Type type = value.GetType();

            //determine the values
            _Value parsed = new _Value(append, type);
            object result = value;
            if (parsed.Signed is long)
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            else if (parsed.Unsigned is ulong)
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>Removes an enumerated type and returns the new value</summary>
        public static T Remove<T>(this Enum value, T remove)
        {
            Type type = value.GetType();

            //determine the values
            _Value parsed = new _Value(remove, type);
            object result = value;
            if (parsed.Signed is long)
                result = Convert.ToInt64(value) & ~(long)parsed.Signed;
            else if (parsed.Unsigned is ulong)
                result = Convert.ToUInt64(value) & ~(ulong)parsed.Unsigned;

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>Checks if an enumerated type is missing a value</summary>
        public static bool MissingFlag<T>(this T obj, Enum value) where T : struct, IConvertible
            => !(Enum.Parse(typeof(T), obj.ToString()) as Enum).HasFlag(value);

        //class to simplfy narrowing values between a ulong and long since either value should cover any lesser value
        private class _Value
        {
            //cached comparisons for tye to use
            private static Type _UInt64 = typeof(ulong);
            private static Type _UInt32 = typeof(long);

            public long? Signed;
            public ulong? Unsigned;

            public _Value(object value, Type type)
            {
                //make sure it is even an enum to work with
                if (!type.IsEnum)
                    throw new ArgumentException("Value provided is not an enumerated type!");

                //then check for the enumerated value
                Type compare = Enum.GetUnderlyingType(type);

                //if this is an unsigned long then the only value that can hold it would be a ulong
                if (compare.Equals(_UInt32) || compare.Equals(_UInt64))
                    Unsigned = Convert.ToUInt64(value);
                //otherwise, a long should cover anything else
                else
                    Signed = Convert.ToInt64(value);
            }
        }
        #endregion
    }
}
