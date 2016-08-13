using System;
using System.Collections.Generic;
using System.Linq;

namespace NBully.Tests.TestInfrastructure
{
	public class InMemoryCommunicator : IBullyCommunicator
	{
		private readonly InMemoryBroker _broker;

		private Action<int> _receiveStartElection;
		private Action<int, int> _receiveAlive;
		private Action<int> _receiveWin;

		public InMemoryCommunicator(InMemoryBroker broker)
		{
			_broker = broker;
			_broker.Communicators.Add(this);
		}

		private List<InMemoryCommunicator> ToOthers => _broker.Communicators.Except(new[] { this }).ToList();

		public void StartElection(int processID)
		{
			ToOthers.ForEach(c => c._receiveStartElection(processID));
		}

		public void SendAlive(int processID, int toProcessID)
		{
			ToOthers.ForEach(c => c._receiveAlive(processID, toProcessID));
		}

		public void BroadcastWin(int processID)
		{
			ToOthers.ForEach(c => c._receiveWin(processID));
		}

		public void OnReceivedStartElection(Action<int> handler)
		{
			_receiveStartElection = handler;
		}

		public void OnReceivedAlive(Action<int, int> handler)
		{
			_receiveAlive = handler;
		}

		public void OnReceivedWin(Action<int> handler)
		{
			_receiveWin = handler;
		}
	}
}