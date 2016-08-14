using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;

namespace NBully.Tests.TestInfrastructure
{
	public class Connector
	{
		private readonly IBullyCommunicator _connector;
		private Action<int> _sendStartElection;
		private Action<int> _sendAlive;
		private Action<int> _sendWin;

		private readonly ManualResetEvent _onWin;

		public Connector()
		{
			_onWin = new ManualResetEvent(false);
			_connector = Substitute.For<IBullyCommunicator>();

			_connector
				.When(c => c.OnReceivedStartElection(Arg.Any<Action<int>>()))
				.Do(ci => _sendStartElection = ci.Arg<Action<int>>());

			_connector
				.When(c => c.OnReceivedAlive(Arg.Any<Action<int>>()))
				.Do(ci => _sendAlive = ci.Arg<Action<int>>());

			_connector
				.When(c => c.OnReceivedWin(Arg.Any<Action<int>>()))
				.Do(ci => _sendWin = ci.Arg<Action<int>>());

			_connector
				.When(c => c.BroadcastWin())
				.Do(ci => _onWin.Set());
		}

		public IBullyCommunicator Communicator => _connector;

		public void SendStartElection(int fromPid)
		{
			_sendStartElection(fromPid);
		}

		public void SendAlive(int fromPid)
		{
			_sendAlive(fromPid);
		}

		public void SendWin(int fromPid)
		{
			_sendWin(fromPid);
		}

		public void WaitForWin(TimeSpan timeout)
		{
			_onWin.WaitOne(timeout);
			_connector.Received().BroadcastWin();
		}
	}
}
