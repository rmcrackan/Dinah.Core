using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Dinah.Core.DataBinding
{
    public class PropertyComparer<T> : IComparer<T>
    {
        private IComparer comparer { get; }
        private PropertyDescriptor propertyDescriptor;
        private int reverse;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            propertyDescriptor = property;
            Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
            comparer = (IComparer)comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
            SetListSortDirection(direction);
        }

        public int Compare(T x, T y) => reverse * comparer.Compare(propertyDescriptor.GetValue(x), propertyDescriptor.GetValue(y));

        public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
        {
            SetPropertyDescriptor(descriptor);
            SetListSortDirection(direction);
        }

        private void SetPropertyDescriptor(PropertyDescriptor descriptor) => propertyDescriptor = descriptor;

        private void SetListSortDirection(ListSortDirection direction) => reverse = direction == ListSortDirection.Ascending ? 1 : -1;
    }
}
