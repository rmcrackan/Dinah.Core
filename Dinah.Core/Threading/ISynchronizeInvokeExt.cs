using System;
using System.ComponentModel;

namespace Dinah.Core.Threading
{
	public static class ISynchronizeInvokeExt
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        public static void UIThread(this ISynchronizeInvoke control, Action code)
        {
            if (control.InvokeRequired)
                control.BeginInvoke(code, null);
            else
                code();
        }
        // alternate version
        // usage:
        //   object1.InvokeIfRequired(c => { c.Visible = true; });
        public static void InvokeIfRequired<T>(this T c, Action<T> action) where T : ISynchronizeInvoke
        {
            if (c.InvokeRequired)
                c.Invoke(new Action(() => action(c)), null);
            else
                action(c);
        }

    }
}
