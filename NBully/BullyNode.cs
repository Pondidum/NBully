using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NBully
{
	public class BullyNode
	{
		private readonly int _id;
		private readonly Action<bool> _onCoordinatorChanged;
		private readonly IBullyCommunicator _messages;
		private readonly HashSet<int> _knownProcesses;
		private readonly CancellationTokenSource _electionTimeout;
		private readonly TimeSpan _timeout;

		private int _coordinator;


		public BullyNode(BullyConfig config)
		{
			_coordinator = 0;
			_knownProcesses = new HashSet<int>();
			_electionTimeout = new CancellationTokenSource();

			_id = config.GetProcessID();
			_timeout = config.Timeout;
			_onCoordinatorChanged = config.OnCoordinatorChanged;
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
				ChangeCoordinator(sourcePid);
		}

		private void ElectionTimeout()
		{
			Task.Delay(_timeout, _electionTimeout.Token).Wait(_electionTimeout.Token);

			if (_electionTimeout.IsCancellationRequested)
				return;

			if (_knownProcesses.Any())
				return;

			_messages.BroadcastWin();
			ChangeCoordinator(_id);
		}

		private void ChangeCoordinator(int coordinatorProcessID)
		{
			var previous = _coordinator;
			_coordinator = coordinatorProcessID;

			if (_coordinator != previous)
				_onCoordinatorChanged(IsCoordinator);
		}
	}
}
