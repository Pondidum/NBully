using System;

namespace NBully
{
	public interface IBullyCommunicator
	{
		int OwnerProcessID { set; }

		void StartElection();
		void SendAlive(int toProcessID);
		void BroadcastWin();

		void OnReceivedStartElection(Action<int> handler);
		void OnReceivedAlive(Action<int> handler);
		void OnReceivedWin(Action<int> handler);

	}
}
