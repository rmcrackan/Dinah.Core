using System.Collections.Generic;

namespace Dinah.Core.DataBinding
{
    public static class ListExt_SortableBindingList
    {
        public static SortableBindingList<T> ToSortableBindingList<T>(this IEnumerable<T> collection) => new SortableBindingList<T>(collection);
    }
}
