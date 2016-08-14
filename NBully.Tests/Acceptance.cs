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
				Timeout = TimeSpan.FromSeconds(2)
			});
		}

		[Fact]
		public void When_the_current_node_triggers_the_election_and_nothing_responds()
		{
			_connector.SendStartElection(100);

			_connector.Communicator.DidNotReceive().SendAlive(100);
			_connector.WaitForWin(TimeSpan.FromSeconds(5));
		}

		[Fact]
		public void When_a_lower_node_triggers_an_election()
		{
			_connector.SendStartElection(50);

			_connector.Communicator.Received().SendAlive(50);
			_connector.WaitForWin(TimeSpan.FromSeconds(5));
		}

		[Fact]
		public void When_a_higher_node_triggers_an_election()
		{
			_connector.SendStartElection(150);

			_connector.WaitForNoWin(TimeSpan.FromSeconds(5));
		}

		[Fact]
		public void When_a_lower_node_sends_alive()
		{
			_connector.SendStartElection(100);
			_connector.SendAlive(50);

			_connector.WaitForWin(TimeSpan.FromSeconds(5));
		}

		[Fact]
		public void When_a_higher_node_sends_alive()
		{
			_connector.SendStartElection(100);
			_connector.SendAlive(150);

			_connector.WaitForNoWin(TimeSpan.FromSeconds(5));
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
			_messages.OnReceivedAlive(OnAlive);
		}

		private void OnStartElection(int sourcePid)
		{
			if (sourcePid > _id)
				_knownProcesses.Add(sourcePid);

			if (_id > sourcePid)
				_messages.SendAlive(sourcePid);

			Task.Run(() => ElectionTimeout());
		}

		private void OnAlive(int sourcePid)
		{
			if (sourcePid > _id)
				_knownProcesses.Add(sourcePid);
		}

		private void ElectionTimeout()
		{
			Thread.Sleep(_config.Timeout);

			if (_knownProcesses.Any() == false)
				_messages.BroadcastWin();
		}
	}
}
