using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dinah.Core.Windows.Forms
{
    /*
     * async form update discussions
     * https://stackoverflow.com/questions/661561/how-do-i-update-the-gui-from-another-thread-in-c
     * https://stackoverflow.com/questions/142003/cross-thread-operation-not-valid-control-accessed-from-a-thread-other-than-the
     */

    public static class ControlExt
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="code"></param>
        public static void UIThread(this Control control, Action code)
        {
            // from: https://stackoverflow.com/a/3588137

            if (control.InvokeRequired)
                control.BeginInvoke(code);
            else
                code.Invoke();
        }
        // alternate version
        // usage:
        //   object1.InvokeIfRequired(c => { c.Visible = true; });
        public static void InvokeIfRequired<T>(this T c, Action<T> action) where T : Control
        {
            if (c.InvokeRequired)
                c.Invoke(new Action(() => action(c)));
            else
                action(c);
        }

        public static List<Control> GetControlListRecursive(this Control.ControlCollection controlCollection)
        {
            var resultCollection = new List<Control>();
            void getControlListRecurs(Control.ControlCollection col)
            {
                foreach (Control child in col)
                {
                    resultCollection.Add(child);
                    getControlListRecurs(child.Controls);
                }
            }
            getControlListRecurs(controlCollection);
            return resultCollection;
        }
    }
}
