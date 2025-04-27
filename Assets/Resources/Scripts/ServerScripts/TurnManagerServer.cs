using System.Linq;
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
				else CallHandler.StartCalling();
				_fusionManager.CurrentTurnStage = TurnStage.Call;
				_fusionManager.ExposingPlayerIx = -1;
				_fusionManager.TurnPlayerIx = playerIx;
				return;
			}

			Debug.Log("Turn Manager server: Discard is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerIx, true); // if not valid, have their discard move back
			return;

			bool ValidateDiscard() => ((_fusionManager.CurrentTurnStage is TurnStage.Discard &&
			                            _fusionManager.TurnPlayerIx == playerIx) ||
			                           (_fusionManager.CurrentTurnStage is TurnStage.Expose &&
			                            _fusionManager.ExposingPlayerIx == playerIx)) &&
			                          _tileTracker.GetTileLoc(tileId) == _tileTracker.GetPrivateRackForPlayer(playerIx);
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
				return;
			}
			
			Debug.Log("Turn Manager server: Pick up is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerIx, true); // if not valid, have their discard move back
			return;

			bool ValidatePickUp() => _fusionManager.CurrentTurnStage is TurnStage.PickUp &&
			                         _fusionManager.TurnPlayerIx == playerIx;
		}

		public void DoNeverMind(int playerIx)
		{
			if (ValidateNeverMind())
			{
				Debug.Log("Turn Manager server: Never Mind is valid - returning tile to discard");
				// TODO: make visual indication that never mind is happening
				_tileTracker.MoveTile(DiscardTileId, SLoc.Discard);
				CallHandler.StartCalling();
				_fusionManager.CurrentTurnStage = TurnStage.Call;
				_tileTracker.SendGameStateToAll();
			}
			Debug.Log("Turn Manager server: Never Mind is NOT valid");
			_tileTracker.SendGameStateToPlayer(playerIx, true);
			return;
				
			bool ValidateNeverMind()
			{
				// checks that the only tile exposed so far is the discard tile
				return _fusionManager.CurrentTurnStage is TurnStage.Expose
				       && playerIx == ExposingPlayerIx
				       && _tileTracker.GetLocContents(_tileTracker.GetDisplayRackForPlayer(ExposingPlayerIx)).Last()
				       == DiscardTileId;
			}
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
	}
}