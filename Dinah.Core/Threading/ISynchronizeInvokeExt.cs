using System;
using System.ComponentModel;

namespace Dinah.Core.Threading
{
	public static class ISynchronizeInvokeExt
    {
        /// <summary>
        /// Executes the Action synchronously on the UI thread. Blocks execution on the calling thread.
        /// </summary>
        public static void UIThreadSync(this ISynchronizeInvoke synchronizeInvoke, Action code)
        {
            if (synchronizeInvoke.InvokeRequired)
                synchronizeInvoke.Invoke(code, null);
            else
                code();
        }

        /// <summary>
        /// Executes the Action asynchronously on the UI thread. Does not block execution on the calling thread.
        /// </summary>
        public static void UIThreadAsync(this ISynchronizeInvoke synchronizeInvoke, Action code)
        {
            if (synchronizeInvoke.InvokeRequired)
                synchronizeInvoke.BeginInvoke(code, null);
            else
                code();
        }

		#region marked obsolete 2021-09-03
        // alternate version
        // usage:
        //   object1.InvokeIfRequired(c => { c.Visible = true; });
        [Obsolete("Obsolete. Use UIThreadSync instead.", true)]
        public static void InvokeIfRequired<T>(this T c, Action<T> action) where T : ISynchronizeInvoke
        {
            if (c.InvokeRequired)
                c.Invoke(new Action(() => action(c)), null);
            else
                action(c);
        }

        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        [Obsolete("Obsolete. Use UIThreadAsync instead.", true)]
        public static void UIThread(this ISynchronizeInvoke control, Action code)
        {
            if (control.InvokeRequired)
                control.BeginInvoke(code, null);
            else
                code();
        }
        #endregion
    }
}
