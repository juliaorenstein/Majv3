using System.Collections.Generic;
using System.Linq;
using Fusion;

namespace Resources
{
	public class CallHandler : NetworkBehaviour
	{
		public TurnManagerServer TurnManager;
		
		private bool _callingPeriod;
		private bool _confirmingPeriod;
		private bool _waitingForJoker;
		private TickTimer _tickTimer;
		private int discardPlayerIx;

		public List<int> PlayersCalling; // players who have pressed the call button
		public List<int> PlayersConfirmed; // players who have pressed the confirm button

		public void StartCalling() => _callingPeriod = true;
		public void WaitForJoker() => _waitingForJoker = true;
		
		public override void FixedUpdateNetwork()
		{
			// quit out if nothing is going on right now
			if (!(_callingPeriod || _confirmingPeriod || _waitingForJoker)) return; 
			
			// if waiting for callers (or joker turn), don't do anything right now
			if (!_tickTimer.ExpiredOrNotRunning(Runner)) return;
			
			// if the timer expired and nobody called, move on to process calls / next turn
			if (_tickTimer.Expired(Runner)) 
			{
				// no callers, next turn
				if (PlayersCalling.Count == 0 || _waitingForJoker)
				{
					NoCallersNextTurn();
					return;
				}
				// otherwise, wait until callers have decided
				_callingPeriod = false;
				_confirmingPeriod = true;
				_tickTimer = TickTimer.None;
			}
			if (_confirmingPeriod)
			{
				if (PlayersCalling.Count == 0) ProcessCalls();
				return;
			}
			// if we made it here, we just started the calling or joker period. Set up timer.
			_tickTimer = TickTimer.CreateFromSeconds(Runner, 2);
		}

		private void NoCallersNextTurn()
		{
			// reset fields
			_callingPeriod = false;
			_waitingForJoker = false;
			_tickTimer = TickTimer.None;
			
			// next turn
			TurnManager.StartNextTurn();
		}
		
		private void ProcessCalls()
		{
			// reset fields
			_confirmingPeriod = false;
			_tickTimer = TickTimer.None;
			
			// if all callers canceled, next turn
			if (PlayersConfirmed.Count == 0)
			{
				NoCallersNextTurn();
				return;
			}
			
			// find the first caller and start expose turn
			// BUG: choosing wrong one when multiple
			int winner = PlayersConfirmed.OrderBy(candidate => (candidate - discardPlayerIx + 4) % 4).First();
			PlayersConfirmed.Clear();
			TurnManager.StartExposeTurn(winner);
		}
	}
}