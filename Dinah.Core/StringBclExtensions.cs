using System.Globalization;

#nullable enable
namespace Dinah.Core
{
    /// <summary>
    /// Extension methods that mirror the public static methods of <see cref="string"/>.
    /// </summary>
    public static class StringBclExtensions
    {
        /// <summary>Compares substrings of two specified System.String objects, ignoring or honoring their case, and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to use in the comparison. </param>
        /// <param name="indexA">The position of the substring within strA. </param>
        /// <param name="strB">The second string to use in the comparison. </param>
        /// <param name="indexB">The position of the substring within strB. </param>
        /// <param name="length">The maximum number of characters in the substrings to compare. </param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false. </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.  Value  Condition Less than zero  The substring in strA precedes the substring in strB in the sort order. Zero  The substrings occur in the same position in the sort order, or length is zero. Greater than zero  The substring in strA follows the substring in strB in the sort order.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">indexA is greater than strA.System.String.Length. -or- indexB is greater than strB.System.String.Length. -or- indexA, indexB, or length is negative. -or- Either indexA or indexB is null, and length is greater than zero.</exception>
        public static int Compare(this string? strA, int indexA, string? strB, int indexB, int length, bool ignoreCase) =>
            string.Compare(strA, indexA, strB, indexB, length, ignoreCase);

        /// <summary>Compares substrings of two specified System.String objects, ignoring or honoring their case and using culture-specific information to influence the comparison, and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to use in the comparison. </param>
        /// <param name="indexA">The position of the substring within strA. </param>
        /// <param name="strB">The second string to use in the comparison. </param>
        /// <param name="indexB">The position of the substring within strB. </param>
        /// <param name="length">The maximum number of characters in the substrings to compare. </param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false. </param>
        /// <param name="culture">An object that supplies culture-specific comparison information. If culture is null, the current culture is used. </param>
        /// <returns>An integer that indicates the lexical relationship between the two comparands.   Value  Condition Less than zero  The substring in strA precedes the substring in strB in the sort order. Zero  The substrings occur in the same position in the sort order, or length is zero. Greater than zero  The substring in strA follows the substring in strB in the sort order.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">indexA is greater than strA.System.String.Length. -or- indexB is greater than strB.System.String.Length. -or- indexA, indexB, or length is negative. -or- Either strA or strB is null, and length is greater than zero.</exception>
        public static int Compare(this string? strA, int indexA, string? strB, int indexB, int length, bool ignoreCase, CultureInfo? culture) =>
            string.Compare(strA, indexA, strB, indexB, length, ignoreCase, culture);

        /// <summary>Compares substrings of two specified System.String objects using the specified comparison options and culture-specific information to influence the comparison, and returns an integer that indicates the relationship of the two substrings to each other in the sort order.</summary>
        /// <param name="strA">The first string to use in the comparison. </param>
        /// <param name="indexA">The starting position of the substring within strA. </param>
        /// <param name="strB">The second string to use in the comparison. </param>
        /// <param name="indexB">The starting position of the substring within strB. </param>
        /// <param name="length">The maximum number of characters in the substrings to compare. </param>
        /// <param name="culture">An object that supplies culture-specific comparison information. If culture is null, the current culture is used. </param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).  </param>
        /// <returns>An integer that indicates the lexical relationship between the two substrings, as shown in the following table.  Value  Condition Less than zero  The substring in strA precedes the substring in strB in the sort order. Zero  The substrings occur in the same position in the sort order, or length is zero. Greater than zero  The substring in strA follows the substring in strB in the sort order.</returns>
        /// <exception cref="System.ArgumentException">options is not a System.Globalization.CompareOptions value.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">indexA is greater than strA.Length. -or- indexB is greater than strB.Length. -or- indexA, indexB, or length is negative. -or- Either strA or strB is null, and length is greater than zero.</exception>
        public static int Compare(this string? strA, int indexA, string? strB, int indexB, int length, CultureInfo? culture, CompareOptions options) =>
            string.Compare(strA, indexA, strB, indexB, length, culture, options);

        /// <summary>Compares substrings of two specified System.String objects using the specified rules, and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to use in the comparison. </param>
        /// <param name="indexA">The position of the substring within strA. </param>
        /// <param name="strB">The second string to use in the comparison. </param>
        /// <param name="indexB">The position of the substring within strB. </param>
        /// <param name="length">The maximum number of characters in the substrings to compare. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.  </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.  Value  Condition Less than zero  The substring in strA precedes the substring in strB in the sort order. Zero  The substrings occur in the same position in the sort order, or the length parameter is zero. Greater than zero  The substring in strA follows the substring in strB in the sort order.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">indexA is greater than strA.System.String.Length. -or- indexB is greater than strB.System.String.Length. -or- indexA, indexB, or length is negative. -or- Either indexA or indexB is null, and length is greater than zero.</exception>
        /// <exception cref="System.ArgumentException">comparisonType is not a System.StringComparison value.</exception>
        public static int Compare(this string? strA, int indexA, string? strB, int indexB, int length, StringComparison comparisonType) =>
            string.Compare(strA, indexA, strB, indexB, length, comparisonType);

        /// <summary>Compares two specified System.String objects and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to compare. </param>
        /// <param name="strB">The second string to compare. </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.  Value  Condition Less than zero strA precedes strB in the sort order. Zero strA occurs in the same position as strB in the sort order. Greater than zero strA follows strB in the sort order.</returns>
        public static int Compare(this string? strA, string? strB) =>
            string.Compare(strA, strB);

        /// <summary>Compares two specified System.String objects, ignoring or honoring their case, and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to compare. </param>
        /// <param name="strB">The second string to compare. </param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false. </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.  Value  Condition Less than zero strA precedes strB in the sort order. Zero strA occurs in the same position as strB in the sort order. Greater than zero strA follows strB in the sort order.</returns>
        public static int Compare(this string? strA, string? strB, bool ignoreCase) =>
            string.Compare(strA, strB, ignoreCase);

        /// <summary>Compares two specified System.String objects, ignoring or honoring their case, and using culture-specific information to influence the comparison, and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to compare. </param>
        /// <param name="strB">The second string to compare. </param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false. </param>
        /// <param name="culture">An object that supplies culture-specific comparison information. If culture is null, the current culture is used. </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.  Value  Condition Less than zero strA precedes strB in the sort order. Zero strA occurs in the same position as strB in the sort order. Greater than zero strA follows strB in the sort order.</returns>
        public static int Compare(this string? strA, string? strB, bool ignoreCase, CultureInfo? culture) =>
            string.Compare(strA, strB, ignoreCase, culture);

        /// <summary>Compares two specified System.String objects using the specified comparison options and culture-specific information to influence the comparison, and returns an integer that indicates the relationship of the two strings to each other in the sort order.</summary>
        /// <param name="strA">The first string to compare. </param>
        /// <param name="strB">The second string to compare. </param>
        /// <param name="culture">The culture that supplies culture-specific comparison information. If culture is null, the current culture is used. </param>
        /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).  </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between strA and strB, as shown in the following table  Value  Condition Less than zero strA precedes strB in the sort order. Zero strA occurs in the same position as strB in the sort order. Greater than zero strA follows strB in the sort order.</returns>
        /// <exception cref="System.ArgumentException">options is not a System.Globalization.CompareOptions value.</exception>
        public static int Compare(this string? strA, string? strB, CultureInfo? culture, CompareOptions options) =>
            string.Compare(strA, strB, culture, options);

        /// <summary>Compares two specified System.String objects using the specified rules, and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to compare. </param>
        /// <param name="strB">The second string to compare. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.  </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.  Value  Condition Less than zero strA precedes strB in the sort order. Zero strA is in the same position as strB in the sort order. Greater than zero strA follows strB in the sort order.</returns>
        /// <exception cref="System.ArgumentException">comparisonType is not a System.StringComparison value.</exception>
        /// <exception cref="System.NotSupportedException">System.StringComparison is not supported.</exception>
        public static int Compare(this string? strA, string? strB, StringComparison comparisonType) =>
            string.Compare(strA, strB, comparisonType);

        /// <summary>Compares substrings of two specified System.String objects and returns an integer that indicates their relative position in the sort order.</summary>
        /// <param name="strA">The first string to use in the comparison. </param>
        /// <param name="indexA">The position of the substring within strA. </param>
        /// <param name="strB">The second string to use in the comparison. </param>
        /// <param name="indexB">The position of the substring within strB. </param>
        /// <param name="length">The maximum number of characters in the substrings to compare. </param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.   Value  Condition Less than zero  The substring in strA precedes the substring in strB in the sort order. Zero  The substrings occur in the same position in the sort order, or length is zero. Greater than zero  The substring in strA follows the substring in strB in the sort order.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">indexA is greater than strA.System.String.Length. -or- indexB is greater than strB.System.String.Length. -or- indexA, indexB, or length is negative. -or- Either indexA or indexB is null, and length is greater than zero.</exception>
        public static int Compare(this string? strA, int indexA, string? strB, int indexB, int length) =>
            string.Compare(strA, indexA, strB, indexB, length);

        /// <summary>Compares substrings of two specified System.String objects by evaluating the numeric values of the corresponding System.Char objects in each substring.</summary>
        /// <param name="strA">The first string to use in the comparison. </param>
        /// <param name="indexA">The starting index of the substring in strA. </param>
        /// <param name="strB">The second string to use in the comparison. </param>
        /// <param name="indexB">The starting index of the substring in strB. </param>
        /// <param name="length">The maximum number of characters in the substrings to compare. </param>
        /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.  Value  Condition Less than zero  The substring in strA is less than the substring in strB. Zero  The substrings are equal, or length is zero. Greater than zero  The substring in strA is greater than the substring in strB.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">strA is not null and indexA is greater than strA.System.String.Length. -or- strB is not null and indexB is greater than strB.System.String.Length. -or- indexA, indexB, or length is negative.</exception>
        public static int CompareOrdinal(this string? strA, int indexA, string? strB, int indexB, int length) =>
            string.CompareOrdinal(strA, indexA, strB, indexB, length);

        /// <summary>Compares two specified System.String objects by evaluating the numeric values of the corresponding System.Char objects in each string.</summary>
        /// <param name="strA">The first string to compare. </param>
        /// <param name="strB">The second string to compare. </param>
        /// <returns>An integer that indicates the lexical relationship between the two comparands.   Value  Condition Less than zero strA is less than strB. Zero strA and strB are equal. Greater than zero strA is greater than strB.</returns>
        public static int CompareOrdinal(this string? strA, string? strB) =>
            string.CompareOrdinal(strA, strB);

        /// <summary>Concatenates four specified instances of System.String.</summary>
        /// <param name="str0">The first string to concatenate. </param>
        /// <param name="str1">The second string to concatenate. </param>
        /// <param name="str2">The third string to concatenate. </param>
        /// <param name="str3">The fourth string to concatenate. </param>
        /// <returns>The concatenation of str0, str1, str2, and str3.</returns>
        public static string Concat(this string? str0, string? str1, string? str2, string? str3) =>
            string.Concat(str0, str1, str2, str3);

        /// <summary>Concatenates three specified instances of System.String.</summary>
        /// <param name="str0">The first string to concatenate. </param>
        /// <param name="str1">The second string to concatenate. </param>
        /// <param name="str2">The third string to concatenate. </param>
        /// <returns>The concatenation of str0, str1, and str2.</returns>
        public static string Concat(this string? str0, string? str1, string? str2) =>
            string.Concat(str0, str1, str2);

        /// <summary>Concatenates two specified instances of System.String.</summary>
        /// <param name="str0">The first string to concatenate. </param>
        /// <param name="str1">The second string to concatenate. </param>
        /// <returns>The concatenation of str0 and str1.</returns>
        public static string Concat(this string? str0, string? str1) =>
            string.Concat(str0, str1);

        public static string Copy(this string str) =>
            string.Copy(str);

        /// <summary>Determines whether two specified System.String objects have the same value. A parameter specifies the culture, case, and sort rules used in the comparison.</summary>
        /// <param name="a">The first string to compare, or null. </param>
        /// <param name="b">The second string to compare, or null. </param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison. </param>
        /// <returns>true if the value of the a parameter is equal to the value of the b parameter; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">comparisonType is not a System.StringComparison value.</exception>
        public static bool Equals(this string? a, string? b, StringComparison comparisonType) =>
            string.Equals(a, b, comparisonType);

        /// <summary>Replaces the format item or items in a specified string with the string representation of the corresponding object. A parameter supplies culture-specific formatting information.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        /// <param name="arg0">The object to format. </param>
        /// <returns>A copy of format in which the format item or items have been replaced by the string representation of arg0.</returns>
        /// <exception cref="System.ArgumentNullException">format is null.</exception>
        /// <exception cref="System.FormatException">format is invalid. -or- The index of a format item is not zero.</exception>
        public static string Format(this string format, IFormatProvider? provider, object? arg0) =>
            string.Format(provider, format, arg0);

        /// <summary>Replaces the format items in a string with the string representations of corresponding objects in a specified array. A parameter supplies culture-specific formatting information.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        /// <param name="args">An object array that contains zero or more objects to format. </param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        /// <exception cref="System.ArgumentNullException">format or args is null.</exception>
        /// <exception cref="System.FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
        public static string Format(this string format, IFormatProvider? provider, params object?[] args) =>
            string.Format(provider, format, args);

        /// <summary>Replaces the format items in a string with the string representations of corresponding objects in a specified span. A parameter supplies culture-specific formatting information.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        /// <param name="args">An object span that contains zero or more objects to format. </param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string Format(this string format, IFormatProvider? provider, params scoped ReadOnlySpan<object?> args) =>
            string.Format(provider, format, args);

        /// <summary>Replaces one or more format items in a string with the string representation of a specified object.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg0">The object to format. </param>
        /// <returns>A copy of format in which any format items are replaced by the string representation of arg0.</returns>
        /// <exception cref="System.ArgumentNullException">format is null.</exception>
        /// <exception cref="System.FormatException">The format item in format is invalid. -or- The index of a format item is not zero.</exception>
        public static string Format(this string format, object? arg0) =>
            string.Format(format, arg0);

        /// <summary>Replaces the format items in a string with the string representation of two specified objects.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg0">The first object to format. </param>
        /// <param name="arg1">The second object to format. </param>
        /// <returns>A copy of format in which format items are replaced by the string representations of arg0 and arg1.</returns>
        /// <exception cref="System.ArgumentNullException">format is null.</exception>
        /// <exception cref="System.FormatException">format is invalid. -or- The index of a format item is not zero or one.</exception>
        public static string Format(this string format, object? arg0, object? arg1) =>
            string.Format(format, arg0, arg1);

        /// <summary>Replaces the format items in a string with the string representation of three specified objects.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg0">The first object to format. </param>
        /// <param name="arg1">The second object to format. </param>
        /// <param name="arg2">The third object to format. </param>
        /// <returns>A copy of format in which the format items have been replaced by the string representations of arg0, arg1, and arg2.</returns>
        /// <exception cref="System.ArgumentNullException">format is null.</exception>
        /// <exception cref="System.FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than two.</exception>
        public static string Format(this string format, object? arg0, object? arg1, object? arg2) =>
            string.Format(format, arg0, arg1, arg2);

        /// <summary>Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="args">An object array that contains zero or more objects to format. </param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        /// <exception cref="System.ArgumentNullException">format or args is null.</exception>
        /// <exception cref="System.FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
        public static string Format(this string format, params object?[] args) =>
            string.Format(format, args);

        /// <summary>Replaces the format items in a string with the string representation of two specified objects. A parameter supplies culture-specific formatting information.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        /// <param name="arg0">The first object to format. </param>
        /// <param name="arg1">The second object to format. </param>
        /// <returns>A copy of format in which format items are replaced by the string representations of arg0 and arg1.</returns>
        /// <exception cref="System.ArgumentNullException">format is null.</exception>
        /// <exception cref="System.FormatException">format is invalid. -or- The index of a format item is not zero or one.</exception>
        public static string Format(this string format, IFormatProvider? provider, object? arg0, object? arg1) =>
            string.Format(provider, format, arg0, arg1);

        /// <summary>Replaces the format item in a specified string with the string representation of a corresponding object in a specified span.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="args">An object span that contains zero or more objects to format. </param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string Format(this string format, params scoped ReadOnlySpan<object?> args) =>
            string.Format(format, args);

        /// <summary>Replaces the format items in a string with the string representation of three specified objects. An parameter supplies culture-specific formatting information.</summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        /// <param name="arg0">The first object to format. </param>
        /// <param name="arg1">The second object to format. </param>
        /// <param name="arg2">The third object to format. </param>
        /// <returns>A copy of format in which the format items have been replaced by the string representations of arg0, arg1, and arg2.</returns>
        /// <exception cref="System.ArgumentNullException">format is null.</exception>
        /// <exception cref="System.FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than two.</exception>
        public static string Format(this string format, IFormatProvider? provider, object? arg0, object? arg1, object? arg2) =>
            string.Format(provider, format, arg0, arg1, arg2);

        /// <summary>Retrieves the system&#x27;s reference to the specified System.String.</summary>
        /// <param name="str">A string to search for in the intern pool. </param>
        /// <returns>The system&#x27;s reference to str, if it is interned; otherwise, a new reference to a string with the value of str.</returns>
        /// <exception cref="System.ArgumentNullException">str is null.</exception>
        public static string Intern(this string str) =>
            string.Intern(str);

        /// <summary>Retrieves a reference to a specified System.String.</summary>
        /// <param name="str">The string to search for in the intern pool. </param>
        /// <returns>A reference to str if it is in the common language runtime intern pool; otherwise, null.</returns>
        /// <exception cref="System.ArgumentNullException">str is null.</exception>
        public static string? IsInterned(this string str) =>
            string.IsInterned(str);

        /// <summary>Indicates whether the specified string is null or an empty string (&quot;&quot;).</summary>
        /// <param name="value">The string to test. </param>
        /// <returns>true if the value parameter is null or an empty string (&quot;&quot;); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string? value) =>
            string.IsNullOrEmpty(value);

        /// <summary>Indicates whether a specified string is null, empty, or consists only of white-space characters.</summary>
        /// <param name="value">The string to test. </param>
        /// <returns>true if the value parameter is null or System.String.Empty, or if value consists exclusively of white-space characters.</returns>
        public static bool IsNullOrWhiteSpace(this string? value) =>
            string.IsNullOrWhiteSpace(value);

        /// <summary>Concatenates the members of a constructed System.Collections.Generic.IEnumerable`1 collection of type System.String, using the specified separator between each member.</summary>
        /// <param name="separator">The string to use as a separator.separator is included in the returned string only if values has more than one element. </param>
        /// <param name="values">A collection that contains the strings to concatenate. </param>
        /// <returns>A string that consists of the elements of values delimited by the separator string. -or- System.String.Empty if values has zero elements.</returns>
        /// <exception cref="System.ArgumentNullException">values is null.</exception>
        /// <exception cref="System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (Int32.MaxValue).</exception>
        public static string Join(this string? separator, IEnumerable<string?> values) =>
            string.Join(separator, values);

        /// <summary>Concatenates the elements of an object array, using the specified separator between each element.</summary>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element. </param>
        /// <param name="values">An array that contains the elements to concatenate. </param>
        /// <returns>A string that consists of the elements of values delimited by the separator string. -or- System.String.Empty if values has zero elements. -or- .NET Framework only: System.String.Empty if the first element of values is null.</returns>
        /// <exception cref="System.ArgumentNullException">values is null.</exception>
        /// <exception cref="System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (Int32.MaxValue).</exception>
        public static string Join(this string? separator, params object?[] values) =>
            string.Join(separator, values);

        /// <summary>Concatenates the string representations of a span of objects, using the specified separator between each member.</summary>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element. </param>
        /// <param name="values">A span of objects whose string representations will be concatenated. </param>
        /// <returns>A string that consists of the elements of values delimited by the separator string. -or- System.String.Empty if values has zero elements.</returns>
        public static string Join(this string? separator, params scoped ReadOnlySpan<object?> values) =>
            string.Join(separator, values);

        /// <summary>Concatenates all the elements of a string array, using the specified separator between each element.</summary>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if value has more than one element. </param>
        /// <param name="value">An array that contains the elements to concatenate. </param>
        /// <returns>A string that consists of the elements in value delimited by the separator string. -or- System.String.Empty if value has zero elements.</returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        /// <exception cref="System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (Int32.MaxValue).</exception>
        public static string Join(this string? separator, params string?[] value) =>
            string.Join(separator, value);

        /// <summary>Concatenates a span of strings, using the specified separator between each member.</summary>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if value has more than one element. </param>
        /// <param name="value">A span that contains the elements to concatenate. </param>
        /// <returns>A string that consists of the elements of value delimited by the separator string. -or- System.String.Empty if value has zero elements.</returns>
        public static string Join(this string? separator, params scoped ReadOnlySpan<string?> value) =>
            string.Join(separator, value);

        /// <summary>Concatenates the specified elements of a string array, using the specified separator between each element.</summary>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if value has more than one element. </param>
        /// <param name="value">An array that contains the elements to concatenate. </param>
        /// <param name="startIndex">The first element in value to use. </param>
        /// <param name="count">The number of elements of value to use. </param>
        /// <returns>A string that consists of count elements of value starting at startIndex delimited by the separator character. -or- System.String.Empty if count is zero.</returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count is less than 0. -or- startIndex plus count is greater than the number of elements in value.</exception>
        /// <exception cref="System.OutOfMemoryException">Out of memory.</exception>
        public static string Join(this string? separator, string?[] value, int startIndex, int count) =>
            string.Join(separator, value, startIndex, count);

        /// <summary>Concatenates the members of a collection, using the specified separator between each member.</summary>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element. </param>
        /// <param name="values">A collection that contains the objects to concatenate.  Type parameters:</param>
        /// <returns>A string that consists of the elements of values delimited by the separator string. -or- System.String.Empty if values has no elements.</returns>
        /// <exception cref="System.ArgumentNullException">values is null.</exception>
        /// <exception cref="System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (Int32.MaxValue).</exception>
        public static string Join<T>(this string? separator, IEnumerable<T> values) =>
            string.Join<T>(separator, values);

    }
}