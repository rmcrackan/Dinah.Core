using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dinah.Core.Diagnostics
{
	[Obsolete("Obsolete. Use: " + nameof(Dinah) + "." + nameof(Core) + "." + nameof(Processes) + "." + nameof(Processes.ProcessResult))]
	public class ProcessResult : Processes.ProcessResult { }

	[Obsolete("Obsolete. Use: " + nameof(Dinah) + "." + nameof(Core) + "." + nameof(Processes) + "." + nameof(Processes.ProcessRunner))]
	public static class ProcessRunner
	{
		[Obsolete("Obsolete. Use: " + nameof(Dinah) + "." + nameof(Core) + "." + nameof(Processes) + "." + nameof(Processes.ProcessRunner) + "." + nameof(Processes.ProcessRunner.WorkingDir))]
		public static string WorkingDir
		{
			get => Processes.ProcessRunner.WorkingDir;
			set => Processes.ProcessRunner.WorkingDir = value;
		}

		[Obsolete("Obsolete. Use: " + nameof(Dinah) + "." + nameof(Core) + "." + nameof(Processes) + "." + nameof(Processes.ProcessRunner) + "." + nameof(Processes.ProcessRunner.RunHidden))]
		public static Processes.ProcessResult RunHidden(this ProcessStartInfo seedInfo) => Processes.ProcessRunner.RunHidden(seedInfo);

		[Obsolete("Obsolete. Use: " + nameof(Dinah) + "." + nameof(Core) + "." + nameof(Processes) + "." + nameof(Processes.ProcessRunner) + "." + nameof(Processes.ProcessRunner.RunHiddenAsync))]
		public static Task<Processes.ProcessResult> RunHiddenAsync(this ProcessStartInfo seedInfo) => Processes.ProcessRunner.RunHiddenAsync(seedInfo);
	}
}
