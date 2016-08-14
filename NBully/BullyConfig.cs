using System;
using System.Diagnostics;

namespace NBully
{
	public class BullyConfig
	{
		public TimeSpan Timeout { get; set; }
		public Func<int> GetProcessID { get; set; }

		public IBullyCommunicator Communicator { get; }

		public BullyConfig(IBullyCommunicator communicator)
		{
			Communicator = communicator;
			Timeout = TimeSpan.FromSeconds(5);
			GetProcessID = () => Process.GetCurrentProcess().Id;
		}
	}
}
