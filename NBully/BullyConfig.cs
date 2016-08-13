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

	public interface IBullyCommunicator
	{
		int OwnerProcessID { set; }

		void StartElection();
		void SendAlive(int toProcessID);
		void BroadcastWin();

		void OnReceivedStartElection(Action<int> handler);
		void OnReceivedAlive(Action<int> handler);
		void OnReceivedWin(Action<int> handler);

	}

	public class BullyNode
	{
		//private readonly BullyConfig _config;
		private readonly int _id;
		private readonly IBullyCommunicator _messages;

		public BullyNode(BullyConfig config)
		{
			_id = config.GetProcessID();
			_messages = config.Communicator;

			_messages.OwnerProcessID = _id;
			_messages.OnReceivedStartElection(OnElectionStarted);
		}

		public void Start()
		{
			_messages.StartElection();
		}

		private void OnElectionStarted(int processID)
		{
			//if (processID < _id)
			//{
			//	_messages.SendAlive(_id, processID);
			//	_messages.StartElection(_id);
			//}
		}
	}
}
