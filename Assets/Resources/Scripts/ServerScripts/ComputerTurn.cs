using System.Linq;
using UnityEngine;

namespace Resources
{
	public class ComputerTurn
	{
		// TODO: the turn happens in 1 frame so the UI doesn't show a tile showing on the computer's private rack. Fix that.
		private readonly TurnManagerServer _turnManager;
		private readonly TileTrackerServer _tileTracker;
		public ComputerTurn(TurnManagerServer turnManager, TileTrackerServer tileTracker)
		{
			_turnManager = turnManager;
			_tileTracker = tileTracker;
		}

		public void FirstTurn(int playerIx)
		{
			Debug.Log($"First turn for player {playerIx}");
			int tileId = _tileTracker.GetLocContents(_tileTracker.GetPrivateRackForPlayer(playerIx)).Last();
			_turnManager.DoDiscard(playerIx, tileId);
		}
		
		public void TakeTurn(int playerIx)
		{
			Debug.Log($"Computer turn for player {playerIx}");
			_turnManager.DoPickUp(playerIx);
			int tileId = _tileTracker.GetLocContents(_tileTracker.GetPrivateRackForPlayer(playerIx)).Last();
			_turnManager.DoDiscard(playerIx, tileId);
		}
	}
}