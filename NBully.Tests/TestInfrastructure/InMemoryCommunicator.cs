using System;
using System.Collections.Generic;
using System.Linq;

namespace NBully.Tests.TestInfrastructure
{
	public class InMemoryCommunicator : IBullyCommunicator
	{
		private readonly InMemoryBroker _broker;

		private Action<int> _receiveStartElection;
		private Action<int> _receiveAlive;
		private Action<int> _receiveWin;

		public InMemoryCommunicator(InMemoryBroker broker)
		{
			_broker = broker;
			_broker.Communicators.Add(this);
		}

		public int OwnerProcessID { get; set; }

		public void StartElection()
		{
			_broker
				.Communicators
				.ForEach(c => c._receiveStartElection(OwnerProcessID));
		}

		public void SendAlive(int toProcessID)
		{
			_broker
				.Communicators
				.Where(other => other.OwnerProcessID == toProcessID)
				.ToList()
				.ForEach(other => other._receiveAlive(OwnerProcessID));
		}

		public void BroadcastWin()
		{
			_broker
				.Communicators
				.Except(new[] { this })
				.ToList()
				.ForEach(c => c._receiveWin(OwnerProcessID));
		}

		public void OnReceivedStartElection(Action<int> handler)
		{
			_receiveStartElection = handler;
		}

		public void OnReceivedAlive(Action<int> handler)
		{
			_receiveAlive = handler;
		}

		public void OnReceivedWin(Action<int> handler)
		{
			_receiveWin = handler;
		}
	}
}