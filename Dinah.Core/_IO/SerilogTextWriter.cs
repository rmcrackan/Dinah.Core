using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dinah.Core.IO
{
	public class SerilogTextWriter : TextWriter
	{
		private const string WRITE = "[WRITE] ";

		public override System.Text.Encoding Encoding => System.Text.Encoding.ASCII;

		public override void WriteLine() => Serilog.Log.Logger.Information("");

		public override void WriteLine(string value) => Serilog.Log.Logger.Information(value);
		public override void WriteLine(bool value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(char value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(decimal value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(double value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(float value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(int value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(long value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(ulong value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(uint value) => Serilog.Log.Logger.Information($"{value}");
		public override void WriteLine(StringBuilder value) => Serilog.Log.Logger.Information($"{value}");

		public override void WriteLine(string format, object arg0) => Serilog.Log.Logger.Information(format, arg0);
		public override void WriteLine(string format, object arg0, object arg1) => Serilog.Log.Logger.Information(format, arg0, arg1);
		public override void WriteLine(string format, object arg0, object arg1, object arg2) => Serilog.Log.Logger.Information(format, arg0, arg1, arg2);
		public override void WriteLine(string format, params object[] arg) => Serilog.Log.Logger.Information(format, arg);

		public override void WriteLine(object value) => Serilog.Log.Logger.Information("{@DebugInfo}", value);

		//
		//// with stack tracing, the 'Write's should be able to handle this correctly.
		//// For now:
		//// - WriteLine.s should be safe
		//// - my lazy solution is to ignore Write. I'll miss sone things but at least impl'ing the above is a net gain
		//

		//public override void Write(object value) => Serilog.Log.Logger.Information(WRITE + "{@DebugInfo}", value);

		//public override void Write(string value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(bool value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(char value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(decimal value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(double value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(float value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(int value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(long value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(uint value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(ulong value) => Serilog.Log.Logger.Information($"{WRITE} {value}");
		//public override void Write(StringBuilder value) => Serilog.Log.Logger.Information($"{WRITE} {value}");

		//public override void Write(string format, object arg0) => Serilog.Log.Logger.Information($"{WRITE} {format}", arg0);
		//public override void Write(string format, object arg0, object arg1) => Serilog.Log.Logger.Information($"{WRITE} {format}", arg0, arg1);
		//public override void Write(string format, object arg0, object arg1, object arg2) => Serilog.Log.Logger.Information($"{WRITE} {format}", arg0, arg1, arg2);
		//public override void Write(string format, params object[] arg) => Serilog.Log.Logger.Information($"{WRITE} {format}", arg);
	}
}
