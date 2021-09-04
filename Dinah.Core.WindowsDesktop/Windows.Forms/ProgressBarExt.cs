using System.Windows.Forms;
using Dinah.Core.Threading;

namespace Dinah.Core.Windows.Forms
{
    public static class ProgressBarExt
    {
        public static void UpdateValue(this ProgressBar progressBar, int value) => progressBar.UIThreadSync(() => progressBar.Value = value);
    }
}
