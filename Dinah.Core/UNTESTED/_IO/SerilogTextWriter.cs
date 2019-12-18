using System;
using System.IO;

namespace Dinah.Core.IO
{
	public class SerilogTextWriter : TextWriter
	{
		public override System.Text.Encoding Encoding => System.Text.Encoding.ASCII;
		public override void WriteLine(string value) => Serilog.Log.Logger.Information(value);
	}
}
