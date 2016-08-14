using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NBully.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NBully.Tests
{
	public class Acceptance
	{
		private readonly ITestOutputHelper _output;
		private readonly Connector _connector;

		public Acceptance(ITestOutputHelper output)
		{
			_output = output;
			_connector = new Connector();
			var node = new Node(new BullyConfig(_connector.Communicator)
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

		[Fact]
		public void When_an_election_is_won_by_a_lower_node()
		{
			_connector.SendStartElection(100);
			_connector.SendWin(50);

			_connector.Communicator.Received().StartElection();
		}

		[Fact]
		public void When_an_election_is_won_by_a_higher_node()
		{
			_connector.SendStartElection(100);
			_connector.SendWin(150);

			_connector.WaitForNoWin(TimeSpan.FromSeconds(5));
		}

		[Fact]
		public void When_an_election_is_won_by_current_node()
		{
			_connector.SendStartElection(100);
			_connector.SendWin(100);

			_connector.WaitForNoWin(TimeSpan.FromSeconds(5));
		}

		[Fact]
		public void When_testing_something()
		{
			var broker = new InMemoryBroker();
			var first = new Node(new BullyConfig(new DebugCommunicator(new InMemoryCommunicator(broker), _output))
			{
				GetProcessID = () => 10,
				Timeout = TimeSpan.FromSeconds(1)
			});

			var second = new Node(new BullyConfig(new DebugCommunicator(new InMemoryCommunicator(broker), _output))
			{
				GetProcessID = () => 20,
				Timeout = TimeSpan.FromSeconds(1)
			});

			var third = new Node(new BullyConfig(new DebugCommunicator(new InMemoryCommunicator(broker), _output))
			{
				GetProcessID = () => 30,
				Timeout = TimeSpan.FromSeconds(1)
			});


			first.Start();

			Thread.Sleep(TimeSpan.FromSeconds(10));

			first.IsCoordinator.ShouldBe(false);
			second.IsCoordinator.ShouldBe(false);
			third.IsCoordinator.ShouldBe(true);
		}
	}

	public class Node
	{
		private readonly BullyConfig _config;
		private readonly int _id;
		private readonly IBullyCommunicator _messages;
		private readonly HashSet<int> _knownProcesses;
		private readonly CancellationTokenSource _electionTimeout;
		private int _coordinator;

		public Node(BullyConfig config)
		{
			_coordinator = 0;
			_knownProcesses = new HashSet<int>();
			_electionTimeout = new CancellationTokenSource();

			_config = config;
			_id = config.GetProcessID();
			_messages = config.Communicator;

			_messages.OwnerProcessID = _id;

			_messages.OnReceivedStartElection(OnStartElection);
			_messages.OnReceivedAlive(OnAlive);
			_messages.OnReceivedWin(OnWin);
		}

		public bool IsCoordinator => _coordinator == _id;

		public void Start()
		{
			_messages.StartElection();
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

		private void OnWin(int sourcePid)
		{
			if (sourcePid > _id)
				_knownProcesses.Add(sourcePid);

			if (sourcePid < _id)
				_messages.StartElection();

			if (sourcePid == _id)
				_electionTimeout.Cancel();

			if (sourcePid >= _id)
				_coordinator = sourcePid;
		}

		private void ElectionTimeout()
		{
			Task.Delay(_config.Timeout, _electionTimeout.Token).Wait(_electionTimeout.Token);

			if (_electionTimeout.IsCancellationRequested)
				return;

			if (_knownProcesses.Any())
				return;

			_messages.BroadcastWin();
			_coordinator = _id;
		}
	}
}
