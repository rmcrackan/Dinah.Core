using System;
using System.Linq;

#nullable enable
namespace Dinah.Core.StepRunner
{
	public class AsyncBasicStep : AsyncBaseStep
	{
		public Func<Task<bool>>? FnT { get; set; }
		protected override async Task<bool> RunRawAsync() => FnT is not null && await FnT();
	}
}
