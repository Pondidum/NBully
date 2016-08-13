﻿using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NBully.Tests.TestInfrastructure
{
	public class InMemoryCommunicatorTests
	{
		private readonly ITestOutputHelper _output;
		private readonly InMemoryBroker _broker;
		private readonly InMemoryCommunicator _first;
		private readonly InMemoryCommunicator _second;
		private readonly InMemoryCommunicator _third;

		public InMemoryCommunicatorTests(ITestOutputHelper output)
		{
			_output = output;
			_broker = new InMemoryBroker();
			_first = new InMemoryCommunicator(_broker);
			_second = new InMemoryCommunicator(_broker);
			_third = new InMemoryCommunicator(_broker);
		}

		[Fact]
		public void When_starting_an_election()
		{
			int? firstReceived = null;
			int? secondReceived = null;
			int? thirdReceived = null;

			_first.OnReceivedStartElection(pid => firstReceived = pid);
			_second.OnReceivedStartElection(pid => secondReceived = pid);
			_third.OnReceivedStartElection(pid => thirdReceived = pid);

			_first.StartElection(1);

			firstReceived.ShouldBeNull();
			secondReceived.ShouldBe(1);
			thirdReceived.ShouldBe(1);
		}

		[Fact]
		public void When_sending_alive()
		{
			int? firstReceived = null;
			int? secondReceived = null;
			int? thirdReceived = null;

			_first.OnReceivedAlive((fromPid, toPid) => firstReceived = toPid);
			_second.OnReceivedAlive((fromPid, toPid) => secondReceived = toPid);
			_third.OnReceivedAlive((fromPid, toPid) => thirdReceived = toPid);

			_first.SendAlive(1, 2);

			firstReceived.ShouldBeNull();
			secondReceived.ShouldBe(2);
			thirdReceived.ShouldBeNull();
		}

		[Fact]
		public void When_sending_win()
		{
			int? firstReceived = null;
			int? secondReceived = null;
			int? thirdReceived = null;

			_first.OnReceivedWin(pid => firstReceived = pid);
			_second.OnReceivedWin(pid => secondReceived = pid);
			_third.OnReceivedWin(pid => thirdReceived = pid);

			_first.BroadcastWin(1);

			firstReceived.ShouldBeNull();
			secondReceived.ShouldBe(1);
			thirdReceived.ShouldBe(1);
		}
	}
}