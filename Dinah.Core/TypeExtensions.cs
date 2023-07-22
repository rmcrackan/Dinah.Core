using System;

#nullable enable
namespace Dinah.Core
{
    public static class TypeExtensions
    {
        public static bool IsGenericOf(this Type type, Type? generic)
        {
            if (generic is null)
                return false;

            var objType = typeof(object);

            Type? toCheck = type;

			while (toCheck != null && toCheck != objType)
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;

				toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
