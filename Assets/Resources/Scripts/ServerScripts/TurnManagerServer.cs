using UnityEngine;

namespace Resources
{
	public class TurnManagerServer
	{
		private readonly TileTrackerServer _tileTracker;
		private readonly FusionManagerGlobal _fusionManager;

		private int _turnPlayerIx = 1;

		public TurnManagerServer(TileTrackerServer tileTracker, FusionManagerGlobal fusionManager)
		{
			_tileTracker = tileTracker;
			_fusionManager = fusionManager;
		}
		
		public void DoDiscard(int playerIx, int tileId)
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

			bool ValidateDiscard() => _turnPlayerIx == playerIx && 
			                          _tileTracker.GetTileLoc(tileId) == _tileTracker.GetPrivateRackForPlayer(playerIx);
			
		}
		
		private void NextTurn()
		{
			Debug.Log("Next turn");
			_turnPlayerIx = (_turnPlayerIx + 1) % 4 + 1;
			_tileTracker.SendGameStateToAll();
		}

		public void DoPickUp(int playerIx)
		{
			if (ValidatePickUp())
			{
				Debug.Log("Turn Manager server: Pick up is valid - picking up");
				_tileTracker.PickupTileWallToRack(playerIx);
				return;
			}
			
			Debug.Log("Turn Manager server: Pick up is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerIx); // if not valid, have their discard move back
			bool ValidatePickUp() => _turnPlayerIx == playerIx &&
			                         _tileTracker.GetLocContents(_tileTracker.GetPrivateRackForPlayer(playerIx)).Count +
			                         _tileTracker.GetLocContents(_tileTracker.GetDisplayRackForPlayer(playerIx)).Count
			                         == 13;
		}
	}
}