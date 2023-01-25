using System;
using System.Linq;

namespace Dinah.Core.StepRunner
{
	public class AsyncBasicStep : AsyncBaseStep
	{
		public Func<Task<bool>> FnT { get; set; }
		protected override async Task<bool> RunRawAsync() => await FnT();
	}
}
