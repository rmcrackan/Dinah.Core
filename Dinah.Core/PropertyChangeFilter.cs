using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Dinah.Core
{
	#region Useage

	/*
	 * USEAGE
	 
		*************************
		*						*
		*   Event Filter Mode   *
		*						*
		*************************

	 
	 	propertyChangeFilter.PropertyChanged += MyPropertiesChanged;

		[PropertyChangeFilter("MyProperty1")]
		[PropertyChangeFilter("MyProperty2")]
		void MyPropertiesChanged(object sender, PropertyChangedEventArgsEx e)
		{
			// Only properties whose names match either "MyProperty1"
			// or "MyProperty2" will fire this event handler.
		}

	******
	* OR *
	******
	
		propertyChangeFilter.PropertyChanged +=
			[PropertyChangeFilter("MyProperty1")]
			[PropertyChangeFilter("MyProperty2")]
			(_, _) => 
			{
				// Only properties whose names match either "MyProperty1"
				// or "MyProperty2" will fire this event handler.
			};


		*************************
		*						*
		*    Observable Mode	*
		*						*
		*************************
		
		using var cancellation = propertyChangeFilter.ObservePropertyChanging<int>("MyProperty", MyPropertyChanging);
		
        void MyPropertyChanging(int oldValue, int newValue)
        {
			// Only the property whose name match
			// "MyProperty" will fire this method.
        }
	
		//The observer is delisted when cancellation is disposed

	******
	* OR *
	******
	
		using var cancellation = propertyChangeFilter.ObservePropertyChanged<bool>("MyProperty", s =>
			{
				// Only the property whose name match
				// "MyProperty" will fire this action.
			});
		
		//The observer is delisted when cancellation is disposed

	 */

	#endregion

	public abstract class PropertyChangeFilter
	{
		private readonly Dictionary<string, List<Delegate>> propertyChangedActions = new();
		private readonly Dictionary<string, List<Delegate>> propertyChangingActions = new();

		/// <summary>
		/// Guards the observer collections above and the lists inside them.
		/// <para>
		/// Registering, unsubscribing and raising all touch the same dictionaries and the same lists, and
		/// nothing stopped two threads from doing so at once. Two registrations for a property no-one had
		/// observed yet both found the key missing and both added it, throwing "an item with the same key
		/// has already been added"; a registration racing an unsubscribe corrupted the list underneath,
		/// which surfaced later as "index was out of range".
		/// </para>
		/// <para>
		/// Held only while the collections are read or written. Subscribers are invoked outside it, over a
		/// snapshot: an observer that changed another property, or that unsubscribed itself, would
		/// otherwise deadlock or invalidate the enumeration it was being called from.
		/// </para>
		/// </summary>
		private readonly object observerLock = new();

		private readonly List<(PropertyChangedEventHandlerEx subscriber, PropertyChangedEventHandlerEx wrapper)> changedFilters = new();
		private readonly List<(PropertyChangingEventHandlerEx subscriber, PropertyChangingEventHandlerEx wrapper)> changingFilters = new();

		protected void OnPropertyChanged(string propertyName, object? newValue)
		{
			//Invoke observables registered for propertyName, over a snapshot taken under the lock so an
			//observer is free to subscribe or unsubscribe while it runs
			foreach (var action in snapshot(propertyChangedActions, propertyName))
				action.DynamicInvoke(newValue);

			_propertyChanged?.Invoke(this, new(propertyName, newValue));
		}

		protected void OnPropertyChanging(string propertyName, object? oldValue, object? newValue)
		{
			//Invoke observables registered for propertyName. See OnPropertyChanged for why a snapshot
			foreach (var action in snapshot(propertyChangingActions, propertyName))
				action.DynamicInvoke(oldValue, newValue);

			_propertyChanging?.Invoke(this, new(propertyName, oldValue, newValue));
		}

		private Delegate[] snapshot(Dictionary<string, List<Delegate>> actions, string propertyName)
		{
			lock (observerLock)
				return actions.TryGetValue(propertyName, out var list) && list is not null
					? list.ToArray()
					: Array.Empty<Delegate>();
		}

		#region Events

		private PropertyChangedEventHandlerEx? _propertyChanged;
		private PropertyChangingEventHandlerEx? _propertyChanging;

		public event PropertyChangedEventHandlerEx PropertyChanged
		{
			add
			{
				var attributes = getAttributes<PropertyChangeFilterAttribute>(value.Method);

				if (attributes.Any())
				{
					var matches = attributes.Select(a => a.PropertyName).ToArray();

					void filterer(object s, PropertyChangedEventArgsEx e)
					{
						if (e.PropertyName.In(matches)) value(s, e);
					}

					changedFilters.Add((value, filterer));

					_propertyChanged += filterer;
				}
				else
					_propertyChanged += value;
			}
			remove
			{
				var del = changedFilters.LastOrDefault(d => d.subscriber == value);
				if (del == default)
					_propertyChanged -= value;
				else
				{
					_propertyChanged -= del.wrapper;
					changedFilters.Remove(del);
				}
			}
		}

		public event PropertyChangingEventHandlerEx PropertyChanging
		{
			add
			{
				var attributes = getAttributes<PropertyChangeFilterAttribute>(value.Method);

				if (attributes.Any())
				{
					var matches = attributes.Select(a => a.PropertyName).ToArray();

					void filterer(object s, PropertyChangingEventArgsEx e)
					{
						if (e.PropertyName.In(matches)) value(s, e);
					}

					changingFilters.Add((value, filterer));

					_propertyChanging += filterer;

				}
				else
					_propertyChanging += value;
			}
			remove
			{
				var del = changingFilters.LastOrDefault(d => d.subscriber == value);
				if (del == default)
					_propertyChanging -= value;
				else
				{
					_propertyChanging -= del.wrapper;
					changingFilters.Remove(del);
				}
			}
		}

		private static T[] getAttributes<T>(MethodInfo methodInfo) where T : Attribute
			=> (T[])Attribute.GetCustomAttributes(methodInfo, typeof(T));

		#endregion

		#region Observables

		/// <summary>
		/// Clear all subscriptions to Property<b>Changed</b> for <paramref name="propertyName"/>
		/// </summary>
		public void ClearChangedSubscriptions(string propertyName)
		{
			lock (observerLock)
				if (propertyChangedActions.TryGetValue(propertyName, out var list) && list is not null)
					list.Clear();
		}

		/// <summary>
		/// Clear all subscriptions to Property<b>Changing</b> for <paramref name="propertyName"/>
		/// </summary>
		public void ClearChangingSubscriptions(string propertyName)
		{
			lock (observerLock)
				if (propertyChangingActions.TryGetValue(propertyName, out var list) && list is not null)
					list.Clear();
		}

		/// <summary>
		/// Add an action to be executed when a property's value has changed
		/// </summary>
		/// <typeparam name="T">The <paramref name="propertyName"/>'s <see cref="Type"/></typeparam>
		/// <param name="propertyName">Name of the property whose change triggers the <paramref name="action"/></param>
		/// <param name="action">Action to be executed with the NewValue as a parameter</param>
		/// <returns>A reference to an interface that allows observers to stop receiving notifications before the provider has finished sending them.</returns>
		public IDisposable ObservePropertyChanged<T>(string propertyName, Action<T> action)
		{
			validateSubscriber<T>(propertyName, action);

			return observe(propertyChangedActions, propertyName, action);
		}

		/// <summary>
		/// Add an action to be executed when a property's value is changing
		/// </summary>
		/// <typeparam name="T">The <paramref name="propertyName"/>'s <see cref="Type"/></typeparam>
		/// <param name="propertyName">Name of the property whose change triggers the <paramref name="action"/></param>
		/// <param name="action">Action to be executed with OldValue and NewValue as parameters</param>
		/// <returns>A reference to an interface that allows observers to stop receiving notifications before the provider has finished sending them.</returns>
		public IDisposable ObservePropertyChanging<T>(string propertyName, Action<T, T> action)
		{
			validateSubscriber<T>(propertyName, action);

			return observe(propertyChangingActions, propertyName, action);
		}

		private IDisposable observe(Dictionary<string, List<Delegate>> actions, string propertyName, Delegate action)
		{
			lock (observerLock)
			{
				if (!actions.TryGetValue(propertyName, out var actionlist))
					actions[propertyName] = actionlist = new List<Delegate>();

				if (!actionlist.Contains(action))
					actionlist.Add(action);

				return new Unsubscriber(observerLock, actionlist, action);
			}
		}

		private void validateSubscriber<T>(string propertyName, Delegate action)
		{
			ArgumentValidator.EnsureNotNullOrWhiteSpace(propertyName, nameof(propertyName));
			ArgumentValidator.EnsureNotNull(action, nameof(action));

			var propertyInfo = GetType().GetProperty(propertyName);

			if (propertyInfo is null)
				throw new MissingMemberException($"{GetType().Name}.{propertyName} does not exist.");

			if (propertyInfo.PropertyType != typeof(T))
				throw new InvalidCastException($"{GetType().Name}.{propertyName} is {propertyInfo.PropertyType}, but parameter is {typeof(T)}.");
		}

		private class Unsubscriber : IDisposable
		{
			private readonly object _lock;
			private List<Delegate> _observers;
			private Delegate _observer;

			internal Unsubscriber(object observerLock, List<Delegate> observers, Delegate observer)
			{
				_lock = observerLock;
				_observers = observers;
				_observer = observer;
			}

			/// <summary>
			/// Removes under the same lock that adds. Removing from a list another thread is adding to
			/// corrupts it, and the damage showed up later, somewhere else, as an index out of range.
			/// </summary>
			public void Dispose()
			{
				lock (_lock)
					_observers.Remove(_observer);
			}
		}

		#endregion
	}

	public delegate void PropertyChangedEventHandlerEx(object sender, PropertyChangedEventArgsEx e);
	public delegate void PropertyChangingEventHandlerEx(object sender, PropertyChangingEventArgsEx e);

	public class PropertyChangedEventArgsEx : PropertyChangedEventArgs
	{
		public object? NewValue { get; }

		public PropertyChangedEventArgsEx(string propertyName, object? newValue) : base(propertyName)
		{
			NewValue = newValue;
		}
	}

	public class PropertyChangingEventArgsEx : PropertyChangingEventArgs
	{
		public object? OldValue { get; }
		public object? NewValue { get; }

		public PropertyChangingEventArgsEx(string propertyName, object? oldValue, object? newValue) : base(propertyName)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class PropertyChangeFilterAttribute : Attribute
	{
		public string PropertyName { get; }
		public PropertyChangeFilterAttribute(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}
