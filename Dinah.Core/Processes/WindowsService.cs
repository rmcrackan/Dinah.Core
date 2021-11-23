using System;
using System.ServiceProcess;

namespace Dinah.Core.Processes
{
    public class WindowsService
	{
		private string serviceProcessName { get; }
		private ServiceController sc { get; }

		public ServiceControllerStatus Status
		{
			get
			{
				sc.Refresh();
				return sc.Status;
			}
		}

		public bool IsRunning => sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending;

		public WindowsService(string serviceProcessName)
		{
			this.serviceProcessName = serviceProcessName;
			sc = new ServiceController(this.serviceProcessName);
		}

		public void Start()
		{
			if (!IsRunning)
				ProcessRunner.RunHidden("sc", $"start {serviceProcessName}");
		}

		public void Stop()
		{
			if (IsRunning)
				ProcessRunner.RunHidden("sc", $"stop {serviceProcessName}");
		}
	}
}
