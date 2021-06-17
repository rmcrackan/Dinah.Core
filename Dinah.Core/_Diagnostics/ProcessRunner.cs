using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dinah.Core.Diagnostics
{
	public class ProcessResult
	{
		List<string> outputLines { get; } = new List<string>();
		List<string> errorLines { get; } = new List<string>();

		public void OutputDataReceived(object sender, DataReceivedEventArgs e) => outputLines.Add(e.Data);
		public void ErrorDataReceived(object sender, DataReceivedEventArgs e) => errorLines.Add(e.Data);

		public int ExitCode { get; set; }

		public string Output => string.Join("\r\n", outputLines);
		public string Error => string.Join("\r\n", errorLines);
	}
	public static class ProcessRunner
    {
        public static string WorkingDir { get; set; } = System.IO.Path.GetDirectoryName(Exe.FileLocationOnDisk);

        public static ProcessResult RunHidden(this ProcessStartInfo seedInfo)
        {
			using var process = new Process { StartInfo = seedInfo };

			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;

			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.UseShellExecute = false;

			if (string.IsNullOrWhiteSpace(process.StartInfo.WorkingDirectory))
				process.StartInfo.WorkingDirectory = WorkingDir;

			var result = new ProcessResult();

			process.OutputDataReceived += result.OutputDataReceived;
			process.ErrorDataReceived += result.ErrorDataReceived;

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();

			result.ExitCode = process.ExitCode;

			process.Close();

			return result;
        }
	}
}
