using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dinah.Core.WindowsDesktop.Processes
{
    public class Executable
	{
		public string ExePath { get; }
		public string ProcessName => System.IO.Path.GetFileNameWithoutExtension(ExePath);

		public bool IsRunning => Process.GetProcessesByName(ProcessName).Any();

		public Executable(string exePath) => ExePath = exePath;

		public void Start(string arguments = "")
		{
			if (!IsRunning)
				Runner.RunHidden(ExePath, arguments);
		}

		public void Kill()
		{
			foreach (var process in Process.GetProcessesByName(ProcessName))
				process.Kill();
		}
	}
}
