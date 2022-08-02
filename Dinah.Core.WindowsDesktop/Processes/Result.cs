using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dinah.Core.WindowsDesktop.Processes
{
    public class Result
	{
		List<string> outputLines { get; } = new List<string>();
		List<string> errorLines { get; } = new List<string>();

		public void OutputDataReceived(object sender, DataReceivedEventArgs e) => logMe(outputLines, e.Data);
		public void ErrorDataReceived(object sender, DataReceivedEventArgs e) => logMe(errorLines, e.Data);
		private static void logMe(List<string> list, string str)
		{
			if (str is not null)
				list.Add(str.Trim('\r', '\n'));
		}

		public int ExitCode { get; set; }

		public string Output => string.Join("\r\n", outputLines);
		public string Error => string.Join("\r\n", errorLines);
	}
}
