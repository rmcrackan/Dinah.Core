using System;

namespace Dinah.Core
{
	// use this in production. use a mock ISystemDateTime for testing
	public class SystemDateTime : ISystemDateTime
	{
		public DateTime Now => DateTime.Now;
		public DateTime UtcNow => DateTime.UtcNow;
	}
}
