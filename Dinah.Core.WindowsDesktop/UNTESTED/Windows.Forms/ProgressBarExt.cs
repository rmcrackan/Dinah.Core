using System.Windows.Forms;

namespace Dinah.Core.Windows.Forms
{
    public static class ProgressBarExt
    {
        public static void UpdateValue(this ProgressBar progressBar, int value) => progressBar.InvokeIfRequired(c => c.Value = value);
    }
}
