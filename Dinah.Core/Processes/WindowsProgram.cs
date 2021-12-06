using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dinah.Core.Processes
{
	public class WindowsProgram
	{
		public string ExePath { get; }
		public string ProcessName => System.IO.Path.GetFileNameWithoutExtension(ExePath);

		public bool IsRunning => Process.GetProcessesByName(ProcessName).Any();

		public WindowsProgram(string exePath) => ExePath = exePath;

		public void Start(string arguments = "")
		{
			if (!IsRunning)
				ProcessRunner.RunHidden(ExePath, arguments);
		}

		public void Kill()
		{
			foreach (var process in Process.GetProcessesByName(ProcessName))
				process.Kill();
		}
	}
}
