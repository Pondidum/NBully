using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NBully.Tests.TestInfrastructure;
using NSubstitute;
using Xunit;

namespace NBully.Tests
{
	public class Acceptance
	{
		private readonly Connector _connector;
		private readonly Node _node;

		public Acceptance()
		{
			_connector = new Connector();
			_node = new Node(new BullyConfig(_connector.Communicator)
			{
				GetProcessID = () => 100,
				Timeout = TimeSpan.FromSeconds(5)
			});
		}

		[Fact]
		public void When_an_election_starts_and_nothing_responds()
		{
			_connector.SendStartElection(100);

			_connector.WaitForWin(TimeSpan.FromSeconds(10));
		}
	}

	public class Node
	{
		private readonly BullyConfig _config;
		private readonly int _id;
		private readonly IBullyCommunicator _messages;
		private readonly HashSet<int> _knownProcesses;

		public Node(BullyConfig config)
		{
			_knownProcesses = new HashSet<int>();

			_config = config;
			_id = config.GetProcessID();
			_messages = config.Communicator;

			_messages.OwnerProcessID = _id;

			_messages.OnReceivedStartElection(OnStartElection);
		}

		private void OnStartElection(int sourcePid)
		{
			Task.Run(() => ElectionTimeout());
		}

		private void ElectionTimeout()
		{
			Thread.Sleep(_config.Timeout);

			if (_knownProcesses.Any() == false)
				_messages.BroadcastWin();
		}
	}
}
