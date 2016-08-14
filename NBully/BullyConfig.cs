using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

	public class BullyNode
	{
		private readonly BullyConfig _config;
		private readonly int _id;
		private readonly IBullyCommunicator _messages;
		private readonly HashSet<int> _knownProcesses;

		public BullyNode(BullyConfig config)
		{
			_config = config;
			_id = config.GetProcessID();
			_messages = config.Communicator;

			_knownProcesses = new HashSet<int>();

			_messages.OwnerProcessID = _id;
			_messages.OnReceivedStartElection(OnElectionStarted);
			_messages.OnReceivedAlive(OnAlive);
		}


		public Task Start()
		{
			_messages.StartElection();

			return Task.Run(() =>
			{
				Thread.Sleep(_config.Timeout);

				if (_knownProcesses.Any() == false)
					_messages.BroadcastWin();
			});
		}

		private void OnElectionStarted(int processID)
		{
			if (processID < _id)
			{
				_messages.SendAlive(processID);
				_messages.StartElection();
			}
		}

		private void OnAlive(int pid)
		{
			if (pid > _id)
				_knownProcesses.Add(pid);
		}
	}
}
