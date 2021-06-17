using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Dinah.Core.Windows.Forms
{
    // from: https://stackoverflow.com/a/18727100

    // use example
    // Console.SetOut(new ControlWriter(textbox1));
    public class TextBoxBaseTextWriter : TextWriter
    {
        private TextBoxBase textbox { get; }
        public TextBoxBaseTextWriter(TextBoxBase textbox) => this.textbox = textbox;

        public override Encoding Encoding => Encoding.ASCII;

        public override void WriteLine(string value) => textbox.UIThread(() => textbox.AppendText(value + Environment.NewLine));
        public override void Write(char value) => textbox.UIThread(() => textbox.AppendText(value.ToString()));
        public override void Write(string value) => textbox.UIThread(() => textbox.AppendText(value));
    }
}
