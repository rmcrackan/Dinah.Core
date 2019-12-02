﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dinah.Core;

namespace Dinah.Core.StepRunner
{
    public abstract class BaseStep
    {
        public string Name { get; set; }

        protected abstract bool RunRaw();

        public (bool IsSuccess, TimeSpan Elapsed) Run()
		{
			logBegin();

			var stopwatch = Stopwatch.StartNew();

			bool success;
			Exception exc = null;
			try
			{
				success = RunRaw();
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

		private void logBegin()
		{
			Serilog.Log.Logger.Debug($"Begin step '{Name}'");
		}

		private void logEnd(bool success, TimeSpan elapsed, Exception exc)
		{
			var logStart = $"End step '{Name}'. ";
			var logEnd = $". Completed in {elapsed.GetTotalTimeFormatted()}";

			if (success)
				Serilog.Log.Logger.Debug($"{logStart}Success{logEnd}");
			else if (exc is null)
				Serilog.Log.Logger.Error($"{logStart}FAILED{logEnd}");
			else
				Serilog.Log.Logger.Error(exc, $"{logStart}FAILED{logEnd}");
		}
	}
}
