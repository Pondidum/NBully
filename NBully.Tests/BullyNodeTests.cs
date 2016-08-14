using System;
using System.Threading.Tasks;
using NBully.Tests.TestInfrastructure;
using NSubstitute;
using Xunit;

namespace NBully.Tests
{
	public class BullyNodeTests
	{
		private readonly Connector _connector;
		private readonly Task _task;

		public BullyNodeTests()
		{
			_connector = new Connector();
			var first = new BullyNode(new BullyConfig(_connector.Communicator)
			{
				GetProcessID = () => 100,
				Timeout = TimeSpan.FromSeconds(2)
			});

			_task = first.Start();
			_connector.Communicator.ClearReceivedCalls();
		}

		[Fact]
		public void When_an_election_is_started_from_a_lower_process()
		{
			_connector.SendStartElection(50);
			_task.Wait();

			_connector.Communicator.Received().SendAlive(50);
			_connector.Communicator.Received().StartElection();
		}

		[Fact]
		public void When_an_election_is_started_from_a_higher_process()
		{
			_connector.SendStartElection(150);
			_task.Wait();

			_connector.Communicator.DidNotReceive().SendAlive(Arg.Any<int>());
			_connector.Communicator.DidNotReceive().StartElection();
		}

		[Fact]
		public void When_a_process_with_a_higher_id_responds()
		{
			_connector.SendAlive(150);
			_task.Wait();

			_connector.Communicator.DidNotReceive().BroadcastWin();
		}

		[Fact]
		public void When_a_process_with_a_lower_id_responds()
		{
			_connector.SendAlive(50);
			_task.Wait();

			_connector.Communicator.Received().BroadcastWin();
		}
	}
}
