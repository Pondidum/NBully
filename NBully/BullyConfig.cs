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
		void StartElection(int processID);
		void SendAlive(int processID, int toProcessID);
		void BroadcastWin(int processID);

		void OnReceivedStartElection(Action<int> handler);
		void OnReceivedAlive(Action<int, int> handler);
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

			_messages.OnReceivedStartElection(OnElectionStarted);
		}

		public void Start()
		{
			_messages.StartElection(_id);
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
