using UnityEngine;

namespace Resources
{
	public class TurnManagerServer
	{
		private readonly TileTrackerServer _tileTracker;
		private readonly FusionManagerGlobal _fusionManager;
		public CallHandler CallHandler;

		private int _turnPlayerIx = 1;
		private int _exposingPlayerIx = -1;
		private int _discardTileId;

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
				_discardTileId = tileId;
				_tileTracker.MoveTile(tileId, SLoc.Discard); // move the tile
				CallHandler.StartCalling(); // TODO: don't do this for jokers
				_fusionManager.CurrentTurnStage = TurnStage.Call;
				return;
			}
			
			Debug.Log("Turn Manager server: Discard is NOT valid");
			_tileTracker.SendGameStateToAll(); // if not valid, have their discard move back

			bool ValidateDiscard() => _turnPlayerIx == playerIx && 
			                          _tileTracker.GetTileLoc(tileId) == _tileTracker.GetPrivateRackForPlayer(playerIx);
		}
		
		public void StartNextTurn()
		{
			Debug.Log("Next turn");
			
			_turnPlayerIx = (_turnPlayerIx + 1) % 4;
			_fusionManager.TurnPlayerIx = _turnPlayerIx;
			_exposingPlayerIx = -1;
			_fusionManager.ExposingPlayerIx = -1;
			_fusionManager.CurrentTurnStage = TurnStage.PickUp;
			_tileTracker.SendGameStateToAll();
			
		}
		
		public void StartExposeTurn(int exposePlayerIx)
		{
			Debug.Log($"Player {exposePlayerIx} called tile {_discardTileId}");
			_exposingPlayerIx = exposePlayerIx;
			_fusionManager.ExposingPlayerIx = exposePlayerIx;
			_fusionManager.CurrentTurnStage = TurnStage.Expose;
			DoExpose(exposePlayerIx, _discardTileId);
		}

		public void DoExpose(int playerIx, int tileId)
		{
			Debug.Log($"Player {playerIx} exposed tile {_exposingPlayerIx}");
			Debug.Assert(playerIx == _exposingPlayerIx);
			_tileTracker.MoveTile(tileId, _tileTracker.GetDisplayRackForPlayer(playerIx));
			_tileTracker.SendGameStateToAll();
		}

		public void DoPickUp(int playerIx)
		{
			if (ValidatePickUp())
			{
				Debug.Log("Turn Manager server: Pick up is valid - picking up");
				_fusionManager.CurrentTurnStage = TurnStage.Discard;
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