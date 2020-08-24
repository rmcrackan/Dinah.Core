using System;

namespace Dinah.Core
{
	public interface IUpdatable
	{
		event EventHandler Updated;
	}
}
