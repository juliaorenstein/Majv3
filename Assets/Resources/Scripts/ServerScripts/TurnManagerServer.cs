using UnityEngine;

namespace Resources
{
	public class TurnManagerServer
	{
		private readonly TileTrackerServer _tileTracker;
		private readonly FusionManagerGlobal _fusionManager;
		private readonly ComputerTurn _computerTurn;
		public CallHandler CallHandler;

		private int TurnPlayerIx => _fusionManager.TurnPlayerIx;
		private int ExposingPlayerIx => _fusionManager.ExposingPlayerIx;
		private int DiscardTileId => _fusionManager.DiscardTileId;

		public TurnManagerServer(TileTrackerServer tileTracker, FusionManagerGlobal fusionManager)
		{
			_tileTracker = tileTracker;
			_fusionManager = fusionManager;
			_computerTurn = new(this, tileTracker);
		}

		public void StartGame()
		{
			Debug.Log("Starting game");
			//_computerTurn.FirstTurn(TurnPlayerIx); // TODO: delete this method or make it work for charleston
		}

		public void DoDiscard(int playerIx, int tileId)
		{
			if (ValidateDiscard()) // validate that this discard is legit
			{
				Debug.Log($"Turn Manager server: Discard is valid - discarding tile {tileId}");
				_fusionManager.DiscardTileId = tileId;
				_tileTracker.MoveTile(tileId, SLoc.Discard); // move the tile
				if (Tile.IsJoker(tileId)) CallHandler.WaitForJoker();
				else CallHandler.StartCalling();
				_fusionManager.CurrentTurnStage = TurnStage.Call;
				_fusionManager.ExposingPlayerIx = -1;
				_fusionManager.TurnPlayerIx = playerIx;
				_fusionManager.numTilesExposedThisTurn = 0;
				return;
			}

			Debug.Log("Turn Manager server: Discard is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerIx, true); // if not valid, have their discard move back
			return;

			bool ValidateDiscard()
			{
				if (_tileTracker.GetTileLoc(tileId) != _tileTracker.GetPrivateRackForPlayer(playerIx)) return false;
				if (_fusionManager.CurrentTurnStage is TurnStage.Discard
				    && _fusionManager.TurnPlayerIx == playerIx) return true;
				return _fusionManager.CurrentTurnStage is TurnStage.Expose
				       && _fusionManager.ExposingPlayerIx == playerIx
				       && _fusionManager.numTilesExposedThisTurn > 2;
			}
		}

		public void DoExpose(int playerIx, int tileId)
		{
			if (ValidateExpose())
			{
				Debug.Log($"Player {playerIx} exposed tile {tileId}");
				Debug.Assert(playerIx == ExposingPlayerIx);
				_fusionManager.numTilesExposedThisTurn += 1;
				_tileTracker.MoveTile(tileId, _tileTracker.GetDisplayRackForPlayer(playerIx));
				_tileTracker.SendGameStateToAll();
				return;
			}
			
			Debug.Log("Turn Manager server: Expose is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerIx, true);
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
				// TODO: do we have a Mah Jongg?
				
				return;
			}
			
			Debug.Log("Turn Manager server: Pick up is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerIx, true); // if not valid, have their discard move back
			return;

			bool ValidatePickUp() => _fusionManager.CurrentTurnStage is TurnStage.PickUp &&
			                         _fusionManager.TurnPlayerIx == playerIx;
		}

		public void StartNextTurn()
		{
			// TODO: maybe make pickup button available but allow calling / don't increment turn until player picks up
			Debug.Log("Next turn");

			_fusionManager.TurnPlayerIx = (TurnPlayerIx + 1) % 4;
			_fusionManager.ExposingPlayerIx = -1;
			_fusionManager.CurrentTurnStage = TurnStage.PickUp;
			_tileTracker.SendGameStateToAll();
			
			if (TurnPlayerIx >= _fusionManager.PlayerCount) _computerTurn.TakeTurn(TurnPlayerIx);
		}
		
		// TODO: do nevermind later

		public void StartExposeTurn(int exposePlayerIx)
		{
			Debug.Log($"Player {exposePlayerIx} called tile {DiscardTileId}");
			_fusionManager.ExposingPlayerIx = exposePlayerIx;
			_fusionManager.CurrentTurnStage = TurnStage.Expose;
			DoExpose(exposePlayerIx, DiscardTileId);
		}
	}
}