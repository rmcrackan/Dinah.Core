using System.Collections.Concurrent;

namespace PropertyChangeFilterTests
{
	/// <summary>
	/// A filter whose observers several threads register and cancel at once.
	/// </summary>
	internal class Subject : PropertyChangeFilter
	{
		private long _speed;
		public long Speed
		{
			get => _speed;
			set { OnPropertyChanging(nameof(Speed), _speed, value); _speed = value; OnPropertyChanged(nameof(Speed), value); }
		}

		private int _count;
		public int Count
		{
			get => _count;
			set { _count = value; OnPropertyChanged(nameof(Count), value); }
		}
	}

	[TestClass]
	public class ObserveFromSeveralThreads
	{
		/// <summary>
		/// Registering and cancelling from several threads used to corrupt the observer collections: two
		/// registrations for a property no-one had observed yet both found the key missing and both added
		/// it, and a registration racing a cancellation damaged the list underneath. The two showed up as
		/// "an item with the same key has already been added" and "index was out of range".
		/// </summary>
		[TestMethod]
		public void registering_and_cancelling_concurrently_does_not_throw()
		{
			var subject = new Subject();
			var failures = new ConcurrentBag<Exception>();

			Parallel.For(0, 500, _ =>
			{
				try
				{
					var registration = subject.ObservePropertyChanged<long>(nameof(Subject.Speed), _ => { });
					registration.Dispose();
				}
				catch (Exception ex) { failures.Add(ex); }
			});

			Assert.AreEqual(0, failures.Count, describe(failures));
		}

		/// <summary>The same for Property<b>Changing</b>, which keeps its own collection.</summary>
		[TestMethod]
		public void registering_changing_observers_concurrently_does_not_throw()
		{
			var subject = new Subject();
			var failures = new ConcurrentBag<Exception>();

			Parallel.For(0, 500, _ =>
			{
				try
				{
					var registration = subject.ObservePropertyChanging<long>(nameof(Subject.Speed), (_, _) => { });
					registration.Dispose();
				}
				catch (Exception ex) { failures.Add(ex); }
			});

			Assert.AreEqual(0, failures.Count, describe(failures));
		}

		/// <summary>
		/// Raising while others subscribe and unsubscribe. Observers are invoked over a snapshot, so the
		/// enumeration cannot be invalidated by a registration arriving mid-notification.
		/// </summary>
		[TestMethod]
		public void raising_while_observers_come_and_go_does_not_throw()
		{
			var subject = new Subject();
			var failures = new ConcurrentBag<Exception>();

			Parallel.For(0, 500, i =>
			{
				try
				{
					if (i % 2 is 0)
					{
						var registration = subject.ObservePropertyChanged<long>(nameof(Subject.Speed), _ => { });
						registration.Dispose();
					}
					else
						subject.Speed = i;
				}
				catch (Exception ex) { failures.Add(ex); }
			});

			Assert.AreEqual(0, failures.Count, describe(failures));
		}

		/// <summary>An observer that unsubscribes itself while being notified must not deadlock.</summary>
		[TestMethod]
		[Timeout(10_000)]
		public void an_observer_can_cancel_itself_while_being_notified()
		{
			var subject = new Subject();
			IDisposable registration = null;
			var notified = 0;

			registration = subject.ObservePropertyChanged<long>(
				nameof(Subject.Speed),
				_ => { notified++; registration.Dispose(); });

			subject.Speed = 1;
			subject.Speed = 2;

			Assert.AreEqual(1, notified, "The observer cancelled itself, so it should not hear the second change.");
		}

		/// <summary>And one that observes a different property from inside a notification.</summary>
		[TestMethod]
		[Timeout(10_000)]
		public void an_observer_can_subscribe_to_another_property_while_being_notified()
		{
			var subject = new Subject();
			IDisposable inner = null;

			using var outer = subject.ObservePropertyChanged<long>(
				nameof(Subject.Speed),
				_ => inner ??= subject.ObservePropertyChanged<int>(nameof(Subject.Count), _ => { }));

			subject.Speed = 1;

			Assert.IsNotNull(inner);
			inner.Dispose();
		}

		/// <summary>The behaviour the locking must not change: an observer hears about the change.</summary>
		[TestMethod]
		public void an_observer_still_hears_the_new_value()
		{
			var subject = new Subject();
			long heard = 0;

			using var registration = subject.ObservePropertyChanged<long>(nameof(Subject.Speed), v => heard = v);

			subject.Speed = 42;

			Assert.AreEqual(42, heard);
		}

		/// <summary>And stops hearing once cancelled.</summary>
		[TestMethod]
		public void a_cancelled_observer_hears_nothing_more()
		{
			var subject = new Subject();
			long heard = 0;

			var registration = subject.ObservePropertyChanged<long>(nameof(Subject.Speed), v => heard = v);
			subject.Speed = 1;
			registration.Dispose();
			subject.Speed = 2;

			Assert.AreEqual(1, heard);
		}

		private static string describe(ConcurrentBag<Exception> failures)
			=> failures.FirstOrDefault() is Exception first
			? $"{failures.Count} failed. First: {first.GetType().Name}: {first.Message}"
			: "";
	}
}
