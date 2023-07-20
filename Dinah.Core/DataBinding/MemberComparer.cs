using System.Collections.Generic;
using System.ComponentModel;

#nullable enable
namespace Dinah.Core.DataBinding
{
	public class MemberComparer<T> : IComparer<T> where T : IMemberComparable
	{
		public ListSortDirection Direction { get; set; } = ListSortDirection.Ascending;
		public string? PropertyName { get; set; }

		public int Compare(T? x, T? y)
		{
			if (x is null && y is null) return 0;
			if (x is null && y is not null) return -1;
			if (x is not null && y is null) return 1;

			if (PropertyName is null)
				throw new NullReferenceException(nameof(PropertyName));

			var val1 = x!.GetMemberValue(PropertyName);
			var val2 = y!.GetMemberValue(PropertyName);

			return DirMult * x.GetMemberComparer(val1.GetType()).Compare(val1, val2);
		}

		private int DirMult => Direction == ListSortDirection.Descending ? -1 : 1;
	}
}
