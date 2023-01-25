using System;
using System.Collections.Generic;
using System.Linq;

namespace Dinah.Core.StepRunner
{
	public class AsyncStepSequence : AsyncBaseStep
	{
		// do NOT use dictionary. order is crucial
		private List<AsyncBaseStep> steps { get; } = new List<AsyncBaseStep>();

		public void Add(AsyncBaseStep step) => steps.Add(step);
		public Func<Task<bool>> this[string name] { set => steps.Add(new AsyncBasicStep { Name = name, FnT = value }); }

		protected override async Task<bool> RunRawAsync()
		{
			foreach (var step in steps)
			{
				var (IsSuccess, Elapsed) = await step.RunAsync();
				if (!IsSuccess)
					return false;
			}

			return true;
		}
	}
}
