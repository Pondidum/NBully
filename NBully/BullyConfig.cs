using System;
using System.Diagnostics;

namespace NBully
{
	public class BullyConfig
	{
		public TimeSpan Timeout { get; set; }
		public Func<int> GetProcessID { get; set; }
		public IBullyCommunicator Communicator { get; set; }

		public BullyConfig()
		{
			Timeout = TimeSpan.FromSeconds(5);
			GetProcessID = () => Process.GetCurrentProcess().Id;
		}
	}
}
