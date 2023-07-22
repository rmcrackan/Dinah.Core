using System;
using System.Diagnostics;

#nullable enable
namespace Dinah.Core.StepRunner
{
	public abstract class AsyncBaseStep : BaseStep
	{
		protected override bool RunRaw() => RunRawAsync().GetAwaiter().GetResult();
		protected abstract Task<bool> RunRawAsync();
		public async Task<(bool IsSuccess, TimeSpan Elapsed)> RunAsync()
		{
			logBegin();

			var stopwatch = Stopwatch.StartNew();

			bool success;
			Exception? exc = null;
			try
			{
				success = await RunRawAsync();
			}
			catch (Exception ex)
			{
				exc = ex;
				success = false;
			}

			stopwatch.Stop();
			var elapsed = stopwatch.Elapsed;

			logEnd(success, elapsed, exc);

			return (success, elapsed);
		}
	}
}
