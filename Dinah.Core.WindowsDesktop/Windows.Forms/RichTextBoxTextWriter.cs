using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Dinah.Core.Threading;

namespace Dinah.Core.Windows.Forms
{
    // from: https://stackoverflow.com/a/18727100
    public class RichTextBoxTextWriter : TextBoxBaseTextWriter
    {
        private RichTextBox richTextBox { get; }
        public RichTextBoxTextWriter(RichTextBox richTextBox) : base(richTextBox) => this.richTextBox = richTextBox;

        public override void WriteLine(string value) => richTextBox?.UIThreadSync(() => writeLine(value));
        private void writeLine(string value)
        {
            if (richTextBox is null || richTextBox.IsDisposed)
                return;

            string text1 = "[" + DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] - " + value + "\r\n";

            Color color = Color.Black;
            if (text1.Contains("WARNING-") || text1.Contains("RED-"))
            {
                text1 = text1.Replace("WARNING-", "").Replace("RED-", "");
                color = Color.Red;
            }
            this.richTextBox.AppendColourText(color, text1);
            this.richTextBox.SelectionStart = this.richTextBox.Text.Length;
            this.richTextBox.ScrollToCaret();
        }

        public override Encoding Encoding => Encoding.ASCII;
    }
}
