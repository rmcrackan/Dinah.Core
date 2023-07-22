using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#nullable enable
namespace Dinah.Core.DataBinding
{
	/// <summary>
	/// <para>Allows filtering of the underlying BindingList<GridEntry> by implementing IBindingListView and using SearchEngineCommands</para>
	/// <para>When filtering is applied, the filtered-out items are removed from the base list and added to the private FilterRemoved list. When filtering is removed, items in the FilterRemoved list are added back to the base list.</para>
	/// <para>Remove is overridden to ensure that removed items are removed from the base list (visible items) as well as the FilterRemoved list.</para>
	/// 
	/// <example>Usage
	/// <code>
	/// FilterableSortableBindingList<GridEntry, LibationSearchEngine.SearchResultSet> bindingList = new(dbBooks.Select(lb => new GridEntry(lb)), nameof(GridEntry.PurchaseDate));
	/// bindingList.GetSearchResults = ApplicationServices.SearchEngineCommands.Search;
	/// bindingList.SearchResultsContain = (searchResults, entry) => searchResults.Docs.Any(r => r.ProductId == entry.AudibleProductId);
	/// </code>
	/// </example>
	/// 
	/// </summary>
	/// <typeparam name="TEntry"></typeparam>
	/// <typeparam name="TSearchResults"></typeparam>
	public class FilterableSortableBindingList<TEntry, TSearchResults> : BindingList<TEntry>, IBindingListView where TEntry : IMemberComparable
	{
		public Func<string?, TSearchResults>? GetSearchResults { get; set; }
		public Func<TSearchResults, TEntry, bool>? SearchResultsContain { get; set; }

		/// <summary>
		/// Items that were removed from the base list due to filtering
		/// </summary>
		private readonly List<TEntry> FilterRemoved = new();
		private string? FilterString;
		private bool isSorted;
		private ListSortDirection listSortDirection;
		private PropertyDescriptor? propertyDescriptor;
		private readonly string DefaultSortProperty;
		private readonly ListSortDirection DefaultSearchDirection;

		public FilterableSortableBindingList(IEnumerable<TEntry> enumeration, string defaultSortProperty, ListSortDirection defaultSearchDirection = ListSortDirection.Descending)
			: base(new List<TEntry>(enumeration))
		{
			DefaultSortProperty = defaultSortProperty;
			DefaultSearchDirection = defaultSearchDirection;

			DefaultSort();
		}

		private MemberComparer<TEntry> Comparer { get; } = new();
		protected override bool SupportsSortingCore => true;
		protected override bool SupportsSearchingCore => true;
		protected override bool IsSortedCore => isSorted;
		protected override PropertyDescriptor? SortPropertyCore => propertyDescriptor;
		protected override ListSortDirection SortDirectionCore => listSortDirection;

		/// <returns>All items in the list, including those filtered out.</returns>
		public List<TEntry> AllItems() => Items.Concat(FilterRemoved).ToList();
		public bool SupportsFiltering => true;
		public string? Filter { get => FilterString; set => ApplyFilter(value); }

		#region Unused - Advanced Filtering
		public bool SupportsAdvancedSorting => false;

		//This ApplySort overload is only called if SupportsAdvancedSorting is true.
		//Otherwise ApplySortCore() is used
		public void ApplySort(ListSortDescriptionCollection sorts) => throw new NotImplementedException();

		public ListSortDescriptionCollection SortDescriptions => throw new NotImplementedException();
		#endregion

		public new void Remove(TEntry entry)
		{
			FilterRemoved.Remove(entry);
			base.Remove(entry);
		}

		protected void DefaultSort()
		{
			Comparer.PropertyName = DefaultSortProperty;
			Comparer.Direction = DefaultSearchDirection;

			Sort();
		}
		protected void Sort()
		{
			List<TEntry> itemsList = (List<TEntry>)Items;

			//Array.Sort() and List<T>.Sort() are unstable sorts. OrderBy is stable.
			var sortedItems = itemsList.OrderBy((ge) => ge, Comparer).ToList();

			itemsList.Clear();
			itemsList.AddRange(sortedItems);
		}

		private void ApplyFilter(string? filterString)
		{
			if (SearchResultsContain is null || GetSearchResults is null)
				throw new NotSupportedException($"{nameof(GetSearchResults)} and {nameof(SearchResultsContain)} must be set before filtering.");

			if (filterString != FilterString)
				RemoveFilter();

			FilterString = filterString;

			var searchResults = GetSearchResults(filterString);

			for (int i = Items.Count - 1; i >= 0; i--)
			{
				if (!SearchResultsContain(searchResults, Items[i]))
				{
					FilterRemoved.Add(Items[i]);
					base.RemoveItem(i);
				}
			}
		}

		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{
			Comparer.PropertyName = property.Name;
			Comparer.Direction = direction;

			Sort();

			propertyDescriptor = property;
			listSortDirection = direction;
			isSorted = true;

			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected override void OnListChanged(ListChangedEventArgs e)
		{
			if (isSorted &&
				e.ListChangedType == ListChangedType.ItemChanged &&
				e.PropertyDescriptor == SortPropertyCore)
			{
				var item = Items[e.NewIndex];
				Sort();
				var newIndex = Items.IndexOf(item);

				base.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, newIndex, e.NewIndex));
			}
			else
				base.OnListChanged(e);
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

			System.Collections.IComparer? valueComparer = null;

			for (int i = 0; i < count; ++i)
			{
				TEntry element = this[i];
				var elemValue = element.GetMemberValue(property.Name);
				valueComparer ??= element.GetMemberComparer(elemValue.GetType());

				if (valueComparer.Compare(elemValue, key) == 0)
				{
					return i;
				}
			}

			return -1;
		}

		public void RemoveFilter()
		{
			if (FilterString is null) return;

			int visibleCount = Items.Count;
			for (int i = 0; i < FilterRemoved.Count; i++)
				base.InsertItem(i + visibleCount, FilterRemoved[i]);

			FilterRemoved.Clear();

			if (IsSortedCore)
				Sort();
			else
				DefaultSort();

			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			FilterString = null;
		}
	}
}
