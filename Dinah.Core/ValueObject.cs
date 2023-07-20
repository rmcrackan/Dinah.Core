using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Dinah.Core
{
	// https://enterprisecraftsmanship.com/2017/08/28/value-object-a-better-implementation/
	public abstract class ValueObject
	{
		protected abstract IEnumerable<object> GetEqualityComponents();

		public override bool Equals(object? obj)
			=> (obj == null || GetType() != obj.GetType())
			? false
			: GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());

		public override int GetHashCode()
		{
			HashCode hash = default;
			foreach (var component in GetEqualityComponents())
				hash.Add(component);
			return hash.ToHashCode();
		}

		public static bool operator ==(ValueObject? a, ValueObject? b)
			=> (a is null && b is null) ? true
			: (a is null || b is null) ? false
			: a.Equals(b);

		public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
	}
}
