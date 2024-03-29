﻿using System;
using System.ComponentModel;
using System.Threading;

#nullable enable
namespace Dinah.Core.Threading
{
	/// <summary>
	/// Captures the current <see cref="SynchronizationContext"/> and allow invoking on that thread.
	/// </summary>
	public class SynchronizeInvoker : ISynchronizeInvoke
	{
		public bool InvokeRequired => AlwaysInvoke || SynchronizationContext.Current != SyncContext || Environment.CurrentManagedThreadId != InstanceThreadId;
		public int InstanceThreadId { get; } = Environment.CurrentManagedThreadId;
		private SynchronizationContext SyncContext { get; }
		private bool AlwaysInvoke { get; }

		public SynchronizeInvoker(bool alwaysInvoke = false)
		{
			SyncContext
				= SynchronizationContext.Current
				?? throw new NullReferenceException($"Could not capture a current {nameof(SynchronizationContext)}");

			AlwaysInvoke = alwaysInvoke;
		}

		public IAsyncResult BeginInvoke(Action action) => BeginInvoke(action, null);
		public IAsyncResult BeginInvoke(Delegate method) => BeginInvoke(method, null);
		public IAsyncResult BeginInvoke(Delegate method, object?[]? args)
		{
			var tme = new ThreadMethodEntry(method, args);

			if (InvokeRequired)
			{
				SyncContext.Post(OnSendOrPostCallback, tme);
			}
			else
			{
				tme.Complete();
				tme.CompletedSynchronously = true;
			}
			return tme;
		}

		public object? EndInvoke(IAsyncResult result)
		{
			if (result is not ThreadMethodEntry crossThread)
				throw new ArgumentException($"{nameof(result)} was not returned by {nameof(SynchronizeInvoker)}.{nameof(BeginInvoke)}");

			if (!crossThread.IsCompleted) 
				crossThread.AsyncWaitHandle.WaitOne();

			return crossThread.ReturnValue;
		}

		public object? Invoke(Action action) => Invoke(action, null);
		public object? Invoke(Delegate method) => Invoke(method, null);
		public object? Invoke(Delegate method, object?[]? args)
		{
			var tme = new ThreadMethodEntry(method, args);

			if (InvokeRequired)
			{
				SyncContext.Send(OnSendOrPostCallback, tme);
			}
			else
			{
				tme.Complete();
				tme.CompletedSynchronously = true;
			}

			return tme.ReturnValue;
		}

		/// <summary>
		/// This callback executes on the SynchronizationContext thread.
		/// </summary>
		private static void OnSendOrPostCallback(object? asyncArgs)
		{
			if (asyncArgs is ThreadMethodEntry e)
				e.Complete();
		}

		private class ThreadMethodEntry : IAsyncResult
		{
			public object? AsyncState => null;
			public bool CompletedSynchronously { get; internal set; }
			public bool IsCompleted { get; private set; }
			public object? ReturnValue { get; private set; }
			public WaitHandle AsyncWaitHandle => completedEvent;

			private Delegate method;
			private object?[]? args;
			private ManualResetEvent completedEvent;

			public ThreadMethodEntry(Delegate method, object?[]? args)
			{
				this.method = method;
				this.args = args;
				completedEvent = new ManualResetEvent(initialState: false);				
			}

			public void Complete()
			{
				try
				{
					switch (method)
					{
						case Action actiton:
							actiton();
							break;
						default:
							ReturnValue = method.DynamicInvoke(args);
							break;
					}
				}
				finally
				{
					IsCompleted = true;
					completedEvent.Set();
				}
			}

			~ThreadMethodEntry()
			{
				completedEvent.Close();
			}
		}
	}
}