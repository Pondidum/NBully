using NBully.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace NBully.Tests
{
	public class Scratchpad
	{
		private readonly ITestOutputHelper _output;

		public Scratchpad(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void When_testing_something()
		{
			//var first = new BullyNode(new BullyConfig()
			//{
			//	Timeout = TimeSpan.FromSeconds(5),
			//	GetProcessID = () => 0
			//});

			//var second = new BullyNode(new BullyConfig
			//{
			//	Timeout = TimeSpan.FromSeconds(5),
			//	GetProcessID = () => 1
			//});

		}
	}
}
