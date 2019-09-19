using System.Drawing;
using System.Windows.Forms;

namespace Dinah.Core.Windows.Forms
{
    public static class RichTextBoxExt
    {
        public static void AppendColourText(this RichTextBox box, Color color, string text)
        {
            int textLength1 = box.TextLength;
            box.AppendText(text);
            int textLength2 = box.TextLength;
            box.Select(textLength1, textLength2 - textLength1);
            box.SelectionColor = color;
            box.SelectionLength = 0;
        }
    }
}
