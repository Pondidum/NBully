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
			var node = new BullyNode(new BullyConfig
			{
				Communicator = _connector.Communicator,
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
			var first = new BullyNode(new BullyConfig
			{
				Communicator = new DebugCommunicator(new InMemoryCommunicator(broker), _output),
				GetProcessID = () => 10,
				Timeout = TimeSpan.FromSeconds(1)
			});

			var second = new BullyNode(new BullyConfig
			{
				Communicator = new DebugCommunicator(new InMemoryCommunicator(broker), _output),
				GetProcessID = () => 20,
				Timeout = TimeSpan.FromSeconds(1)
			});

			var third = new BullyNode(new BullyConfig
			{
				Communicator = new DebugCommunicator(new InMemoryCommunicator(broker), _output),
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
}
