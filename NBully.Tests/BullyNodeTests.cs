using NBully.Tests.TestInfrastructure;
using NSubstitute;
using Xunit;

namespace NBully.Tests
{
	public class BullyNodeTests
	{
		[Fact]
		public void When_an_election_is_started_from_a_lower_process()
		{
			var connector = new Connector();
			var first = new BullyNode(new BullyConfig(connector.Communicator)
			{
				GetProcessID = () => 100
			});

			first.Start();
			connector.Communicator.ClearReceivedCalls();

			connector.SendStartElection(50);
			
			connector.Communicator.Received().SendAlive(50);
			connector.Communicator.Received().StartElection();
		}

		[Fact]
		public void When_an_election_is_started_from_a_higher_process()
		{
			var connector = new Connector();
			var first = new BullyNode(new BullyConfig(connector.Communicator)
			{
				GetProcessID = () => 100
			});

			first.Start();
			connector.Communicator.ClearReceivedCalls();

			connector.SendStartElection(150);

			connector.Communicator.DidNotReceive().SendAlive(Arg.Any<int>());
			connector.Communicator.DidNotReceive().StartElection();
		}
	}
}
