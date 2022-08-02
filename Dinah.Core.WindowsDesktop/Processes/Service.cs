using System;
using System.ServiceProcess;

namespace Dinah.Core.WindowsDesktop.Processes
{
    public class Service
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

		public Service(string serviceProcessName)
		{
			this.serviceProcessName = serviceProcessName;
			sc = new ServiceController(this.serviceProcessName);
		}

		public void Start()
		{
			if (!IsRunning)
				Runner.RunHidden("sc", $"start {serviceProcessName}");
		}

		public void Stop()
		{
			if (IsRunning)
				Runner.RunHidden("sc", $"stop {serviceProcessName}");
		}
	}
}
