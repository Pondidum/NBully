using System.Collections.Generic;

namespace NBully.Tests.TestInfrastructure
{
	public class InMemoryBroker
	{
		public List<InMemoryCommunicator> Communicators { get; } = new List<InMemoryCommunicator>();
	}
}