using UnityEngine;

namespace Resources
{
	public class TurnManagerServer
	{
		private readonly TileTrackerServer _tileTracker;
		private readonly FusionManagerGlobal _fusionManager;
		public CallHandler CallHandler;

		private int TurnPlayerIx => _fusionManager.TurnPlayerIx;
		private int ExposingPlayerIx => _fusionManager.ExposingPlayerIx;
		public int DiscardTileId => _fusionManager.DiscardTileId;

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
				_fusionManager.DiscardTileId = tileId;
				_tileTracker.MoveTile(tileId, SLoc.Discard); // move the tile
				if (Tile.IsJoker(tileId)) CallHandler.WaitForJoker();
				else CallHandler.StartCalling(); // TODO: don't do this for jokers
				_fusionManager.CurrentTurnStage = TurnStage.Call;
				_fusionManager.ExposingPlayerIx = -1;
				_fusionManager.TurnPlayerIx = playerIx;
				return;
			}

			Debug.Log("Turn Manager server: Discard is NOT valid");
			_tileTracker.SendGameStateToAll(); // if not valid, have their discard move back
			return;

			bool ValidateDiscard() => ((_fusionManager.CurrentTurnStage is TurnStage.Discard &&
			                            _fusionManager.TurnPlayerIx == playerIx) ||
			                           (_fusionManager.CurrentTurnStage is TurnStage.Expose &&
			                            _fusionManager.ExposingPlayerIx == playerIx)) &&
			                          _tileTracker.GetTileLoc(tileId) == _tileTracker.GetPrivateRackForPlayer(playerIx);
		}

		public void StartNextTurn()
		{
			Debug.Log("Next turn");

			_fusionManager.TurnPlayerIx = (TurnPlayerIx + 1) % 4;
			_fusionManager.ExposingPlayerIx = -1;
			_fusionManager.CurrentTurnStage = TurnStage.PickUp;
			_tileTracker.SendGameStateToAll();
		}
		
		// TODO: do nevermind later

		public void StartExposeTurn(int exposePlayerIx)
		{
			Debug.Log($"Player {exposePlayerIx} called tile {DiscardTileId}");
			_fusionManager.ExposingPlayerIx = exposePlayerIx;
			_fusionManager.CurrentTurnStage = TurnStage.Expose;
			DoExpose(exposePlayerIx, DiscardTileId);
		}

		public void DoExpose(int playerIx, int tileId)
		{
			if (ValidateExpose())
			{
				Debug.Log($"Player {playerIx} exposed tile {tileId}");
				Debug.Assert(playerIx == ExposingPlayerIx);
				_tileTracker.MoveTile(tileId, _tileTracker.GetDisplayRackForPlayer(playerIx));
				_tileTracker.SendGameStateToAll();
				return;
			}
			
			Debug.Log("Turn Manager server: Expose is NOT valid");
			_tileTracker.SendGameStateToAll();
			return;

			bool ValidateExpose() => _fusionManager.CurrentTurnStage == TurnStage.Expose
			                         && _fusionManager.ExposingPlayerIx == playerIx 
			                         && ((tileId == DiscardTileId && !Tile.IsJoker(tileId))
			                             || _tileTracker.GetTileLoc(tileId) 
			                             == _tileTracker.GetPrivateRackForPlayer(playerIx));
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

			bool ValidatePickUp() => _fusionManager.CurrentTurnStage == TurnStage.PickUp &&
			                         _fusionManager.TurnPlayerIx == playerIx;
		}
	}
}