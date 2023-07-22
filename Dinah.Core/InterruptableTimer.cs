using System;

#nullable enable
namespace Dinah.Core
{
    public class InterruptableTimer
    {
        public event EventHandler? Elapsed;

        private System.Timers.Timer _timer;

        public InterruptableTimer(TimeSpan interval) : this(interval.TotalMilliseconds) { }
        public InterruptableTimer(double intervalInMilliseconds)
        {
            _timer = new() { Interval = intervalInMilliseconds };
            _timer.Elapsed += PerformNow;
        }

        /// <summary>
        /// <para>If timer is not running: invoke event, then begin waiting.</para>
        /// <para>If timer is running: stop timer (ie: do not wait for interval to complete. aka: interrupt), invoke event then, re-start waiting.</para>
        /// </summary>
        public void PerformNow() => PerformNow(null, EventArgs.Empty);

		public void PerformNow(object? _, EventArgs __)
        {
            Stop();
            Elapsed?.Invoke(_, __);
            _timer.Enabled = true;
        }

        public void Stop() => _timer.Enabled = false;
    }
}
