using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dinah.Core.Windows.Forms
{
    public static class FormExt
    {
        /// <summary>
        /// <para>Close the form on the form's thread. Responds immediately.</para>
        /// Dinah.Core.Windows.Forms.UIThread() doesn't respond until the current method is complete. no idea why
        /// </summary>
        /// <param name="form"></param>
        public static void Kill(this Form form)
            => form.Invoke((MethodInvoker)delegate
            {
                form.Close();
                form.Dispose();
            });
    }
}
