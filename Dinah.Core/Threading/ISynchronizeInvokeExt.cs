using System;
using System.ComponentModel;

namespace Dinah.Core.Threading
{
	public static class ISynchronizeInvokeExt
    {
        /// <summary>
        /// Executes the Action synchronously on the UI thread. Blocks execution on the calling thread. (Previously named "InvokeIfRequired")
        /// </summary>
        public static void UIThreadSync(this ISynchronizeInvoke synchronizeInvoke, Action code)
        {
            if (synchronizeInvoke.InvokeRequired)
                synchronizeInvoke.Invoke(code, null);
            else
                code();
        }

        /// <summary>
        /// Executes the Action asynchronously on the UI thread. Does not block execution on the calling thread. (Previously named "UIThread")
        /// </summary>
        public static void UIThreadAsync(this ISynchronizeInvoke synchronizeInvoke, Action code)
        {
            if (synchronizeInvoke.InvokeRequired)
                synchronizeInvoke.BeginInvoke(code, null);
            else
                code();
        }
    }
}
