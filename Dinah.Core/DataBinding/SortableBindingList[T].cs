using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dinah.Core.DataBinding
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private Dictionary<Type, PropertyComparer<T>> comparers { get; } = new Dictionary<Type, PropertyComparer<T>>();
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;

        public SortableBindingList() : base(new List<T>()) { }

        public SortableBindingList(IEnumerable<T> enumeration) : base(new List<T>(enumeration)) { }

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => isSorted;

        protected override PropertyDescriptor SortPropertyCore => propertyDescriptor;

        protected override ListSortDirection SortDirectionCore => listSortDirection;

        protected override bool SupportsSearchingCore => true;

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> itemsList = (List<T>)Items;

            Type propertyType = property.PropertyType;
            if (!comparers.TryGetValue(propertyType, out PropertyComparer<T> comparer))
            {
                comparer = new PropertyComparer<T>(property, direction);
                comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            propertyDescriptor = property;
            listSortDirection = direction;
            isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
            propertyDescriptor = base.SortPropertyCore;
            listSortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {
            int count = Count;
            for (int i = 0; i < count; ++i)
            {
                T element = this[i];
                if (property.GetValue(element).Equals(key))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
