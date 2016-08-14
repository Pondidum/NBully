using System;
using Xunit.Abstractions;

namespace NBully.Tests.TestInfrastructure
{
	public class DebugCommunicator : IBullyCommunicator
	{
		private readonly IBullyCommunicator _other;
		private readonly ITestOutputHelper _output;
		private int _pid;

		public DebugCommunicator(IBullyCommunicator other, ITestOutputHelper output)
		{
			_other = other;
			_output = output;
		}

		public int OwnerProcessID
		{
			set
			{
				_other.OwnerProcessID = value;
				_pid = value;
			}
		}

		public void StartElection()
		{
			_output.WriteLine($"{_pid} Starting Election");
			_other.StartElection();
		}

		public void SendAlive(int toProcessID)
		{
			_output.WriteLine($"{_pid} Sending Alive to {toProcessID}");
			_other.SendAlive(toProcessID);
		}

		public void BroadcastWin()
		{
			_output.WriteLine($"{_pid} Broadcasting Win");
			_other.BroadcastWin();
		}

		public void OnReceivedStartElection(Action<int> handler)
		{
			_other.OnReceivedStartElection(pid =>
			{
				_output.WriteLine($"{_pid} Received StartElection from {pid}");
				handler(pid);
			});
		}

		public void OnReceivedAlive(Action<int> handler)
		{
			_other.OnReceivedAlive(pid =>
			{
				_output.WriteLine($"{_pid} Received Alive from {pid}");
				handler(pid);
			});
		}

		public void OnReceivedWin(Action<int> handler)
		{
			_other.OnReceivedWin(pid =>
			{
				_output.WriteLine($"{_pid} Received Win from {pid}");
				handler(pid);
			});
		}
	}
}