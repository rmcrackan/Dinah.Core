using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dinah.Core.Collections.Immutable
{
	// https://stackoverflow.com/a/48480845
	// thread-safe, immutable, lock free

	// consider replacing with Microsoft.Extensions.Caching.Memory nuget's MemoryCache:
	// https://michaelscodingspot.com/cache-implementations-in-csharp-net/
	public class Cache<T> : IEnumerable<T>
	{
		private ImmutableHashSet<T> cache;

		public Cache()
			=> cache = ImmutableHashSet.Create<T>();

		public Cache(IEnumerable<T> items)
		{
			var builder = ImmutableHashSet.CreateBuilder<T>();
			builder.UnionWith(items);
			cache = builder.ToImmutableHashSet();
		}

		public void Add(T newEntry)
			=> ImmutableInterlocked.Update(ref cache, (set, item) => set.Add(item), newEntry);

		public void Remove(T entryToRemove)
			=> ImmutableInterlocked.Update(ref cache, (set, item) => set.Remove(item), entryToRemove);

		public IEnumerator<T> GetEnumerator()
			=> cache.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> cache.GetEnumerator();
	}
}
