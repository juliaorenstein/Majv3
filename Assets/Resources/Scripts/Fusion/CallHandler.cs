using System.Collections.Generic;
using System.Linq;
using Fusion;

namespace Resources
{
	public class CallHandler : NetworkBehaviour
	{
		public TurnManagerServer TurnManager;
		
		private bool _isCallingPeriod;
		private bool _waitingForJoker;
		private TickTimer _tickTimer;
		private int discardPlayerIx;

		public List<int> PlayersCalling; // players who have pressed the call button
		public List<int> PlayersThinking; // players who have pressed the wait button
		public List<int> PlayersPassing; // players who have pressed the pass button

		public void StartCalling() => _isCallingPeriod = true;
		public void WaitForJoker() => _waitingForJoker = true;
		
		public override void FixedUpdateNetwork()
		{
			if (!(_isCallingPeriod || _waitingForJoker)) return; // quit out if not calling right now
			if (_tickTimer.Expired(Runner)) 
			{
				if (PlayersThinking.Count == 0) ProcessCalls();
				return;
			}
			if (_waitingForJoker && _tickTimer.IsRunning) return;
			// if timer just expired, move to next turn unless anybody is thinking
			// this works for both calling and joker wait
			if (_tickTimer.IsRunning) // if the timer is running, continue, unless players have all made up mind
			{
				if (PlayersPassing.Count + PlayersCalling.Count == 4) ProcessCalls();
				return;
			}
			// if we made it here, we just started the calling or joker period. Set up timer.
			_tickTimer = TickTimer.CreateFromSeconds(Runner, 2);
		}
		
		void ProcessCalls()
		{
			// reset fields
			_isCallingPeriod = false;
			_waitingForJoker = false;
			_tickTimer = TickTimer.None;
			
			// if there are no callers, go to next turn
			if (PlayersCalling.Count == 0)
			{
				ClearAllTrackers();
				TurnManager.StartNextTurn();
				return;
			}
			
			// find the first caller and start expose turn
			// BUG: choosing wrong one when multiple
			int winner = PlayersCalling.OrderBy(candidate => (candidate - discardPlayerIx + 4) % 4).First();
			ClearAllTrackers(); // TODO: never mind logic will require this line to move
			TurnManager.StartExposeTurn(winner);
		}

		void ClearAllTrackers()
		{
			PlayersCalling.Clear();
			PlayersPassing.Clear();
			PlayersThinking.Clear();
		}
	}
}