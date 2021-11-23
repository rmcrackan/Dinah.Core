using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Dinah.Core.Processes
{
    public static class ProcessRunner
    {
        public static string WorkingDir { get; set; } = System.IO.Path.GetDirectoryName(Exe.FileLocationOnDisk);

		public static ProcessResult RunHidden(string name, string arguments)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = name,
				Arguments = arguments
			};
			return RunHidden(processStartInfo);
		}

		public static ProcessResult RunHidden(this ProcessStartInfo seedInfo)
		{
			using var process = new Process { StartInfo = seedInfo };
			var result = new ProcessResult();
			initProcess(process, result);

			process.WaitForExit();

			result.ExitCode = process.ExitCode;
			process.Close();
			return result;
		}

		public static async Task<ProcessResult> RunHiddenAsync(string name, string arguments)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = name,
				Arguments = arguments
			};
			return await RunHiddenAsync(processStartInfo);
		}

		public static async Task<ProcessResult> RunHiddenAsync(this ProcessStartInfo seedInfo)
		{
			using var process = new Process { StartInfo = seedInfo };
			var result = new ProcessResult();
			initProcess(process, result);

			await process.WaitForExitAsync();

			result.ExitCode = process.ExitCode;
			process.Close();
			return result;
		}

		public static bool IsAdministrator()
		{
			if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
				return true;

			var identity = WindowsIdentity.GetCurrent();
			if (identity is null)
				return false;

			return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
		}

		private static void initProcess(Process process, ProcessResult result)
		{
			var startInfo = process.StartInfo;

			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;

			startInfo.CreateNoWindow = true;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.UseShellExecute = false;

			if (string.IsNullOrWhiteSpace(startInfo.WorkingDirectory))
				startInfo.WorkingDirectory = WorkingDir;

			// https://stackoverflow.com/a/3892229
			if (!IsAdministrator())
				startInfo.Verb = "runas";

			process.OutputDataReceived += result.OutputDataReceived;
			process.ErrorDataReceived += result.ErrorDataReceived;

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		}
	}
}
