using UnityEngine;

namespace Resources
{
	public class TurnManagerServer
	{
		private readonly TileTrackerServer _tileTracker;
		private readonly FusionManagerServer _fusionManager;

		private int _turnPlayerId = 0;

		public TurnManagerServer(TileTrackerServer tileTracker, FusionManagerServer fusionManager)
		{
			_tileTracker = tileTracker;
			_fusionManager = fusionManager;
		}
		
		public void DoDiscard(int playerId, int tileId)
		{
			if (ValidateDiscard()) // validate that this discard is legit
			{
				Debug.Log("Turn Manager server: Discard is valid - discarding");
				_tileTracker.MoveTile(tileId, SLoc.Discard); // move the tile
				NextTurn(); // increment the turn
				return;
			}
			
			Debug.Log("Turn Manager server: Discard is NOT valid");
			_tileTracker.SendGameStateToAll(); // if not valid, have their discard move back

			bool ValidateDiscard() => _turnPlayerId == playerId && 
			                          _tileTracker.GetTileLoc(tileId) == _tileTracker.GetPrivateRackForPlayer(playerId);
			
		}
		
		private void NextTurn()
		{
			Debug.Log("Next turn");
			_turnPlayerId = (_turnPlayerId + 1) % 4;
			_tileTracker.SendGameStateToAll();
		}

		public void DoPickUp(int playerId)
		{
			if (ValidatePickUp())
			{
				Debug.Log("Turn Manager server: Pick up is valid - picking up");
				_tileTracker.PickupTileWallToRack(playerId);
				return;
			}
			
			Debug.Log("Turn Manager server: Pick up is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerId); // if not valid, have their discard move back
			bool ValidatePickUp() => _turnPlayerId == playerId &&
			                         _tileTracker.GetLocContents(_tileTracker.GetPrivateRackForPlayer(playerId)).Count +
			                         _tileTracker.GetLocContents(_tileTracker.GetDisplayRackForPlayer(playerId)).Count
			                         == 13;
		}
	}
}