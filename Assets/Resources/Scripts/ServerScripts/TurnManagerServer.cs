using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
	// BUG: can only joker swap on your own rack right now
	// BUG: on joker swap other clients see you have one less private rack tile
	public class TurnManagerServer
	{
		private readonly TileTrackerServer _tileTracker;
		private readonly FusionManagerGlobal _fusionManager;
		private readonly ComputerTurn _computerTurn;
		private readonly CallHandler _callHandler;
		private readonly Card _card;

		private int TurnPlayerIx => _fusionManager.TurnPlayerIx;
		private int ExposingPlayerIx => _fusionManager.ExposingPlayerIx;
		private int DiscardTileId => _fusionManager.DiscardTileId;

		public TurnManagerServer(TileTrackerServer tileTracker, FusionManagerGlobal fusionManager, CallHandler callHandler)
		{
			_tileTracker = tileTracker;
			_fusionManager = fusionManager;
			_computerTurn = new(this, tileTracker);
			_callHandler = callHandler;
			_card = new("/Users/juliaorenstein/Unity/Majv3/Assets/Resources/Scripts/MajLogic/2024Card.txt");
		}

		public void StartGame()
		{
			Debug.Log("Starting game");
			_fusionManager.CurrentTurnStage = TurnStage.Discard;
			if (TurnPlayerIx >= _fusionManager.HumanPlayerCount) _computerTurn.FirstTurn(TurnPlayerIx);
		}

		public void DoDiscard(int playerIx, int tileId)
		{
			if (ValidateDiscard()) // validate that this discard is legit
			{
				Debug.Log($"Turn Manager server: Discard is valid - discarding tile {tileId}");
				_fusionManager.DiscardTileId = tileId;
				_tileTracker.MoveTile(tileId, SLoc.Discard); // move the tile
				if (Tile.IsJoker(tileId)) _callHandler.WaitForJoker();
				else _callHandler.StartCalling();
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
				CheckForMahJongg(playerIx);
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
			
			if (TurnPlayerIx >= _fusionManager.HumanPlayerCount) _computerTurn.TakeTurn(TurnPlayerIx);
		}
		
		// TODO: do nevermind later

		public void StartExposeTurn(int exposePlayerIx)
		{
			Debug.Log($"Player {exposePlayerIx} called tile {DiscardTileId}");
			_fusionManager.ExposingPlayerIx = exposePlayerIx;
			_fusionManager.CurrentTurnStage = TurnStage.Expose;
			if (CheckForMahJongg(exposePlayerIx)) return;
			DoExpose(exposePlayerIx, DiscardTileId);
		}

		public void DoJokerSwap(int playerIx, int tileId, int jokerTileId)
		{
			// validate player is allowed to do joker swap right now
			if (!((_fusionManager.CurrentTurnStage is TurnStage.Discard && playerIx == _fusionManager.TurnPlayerIx)
				    || (_fusionManager.CurrentTurnStage is TurnStage.Expose && playerIx == _fusionManager.ExposingPlayerIx)))
			{
				throw new UnityException("TurnManagerServer: Joker swap player is not valid");
			}
			
			// find the rack the joker is on and its ix
			SLoc displayRack = _tileTracker.GetTileLoc(jokerTileId);
			List<int> displayRackContents = _tileTracker.GetLocContents(displayRack);
			int jokerIx = displayRackContents.IndexOf(jokerTileId);
			
			// validate that the tile to the left of the joker matches tileId
			if (!Tile.AreSame(displayRackContents[jokerIx - 1], tileId))
			{
				throw new UnityException("TurnManagerServer: Joker swap tile doesn't match group.");
			}
			
			// get the private rack the tileId is on
			SLoc privateRack = _tileTracker.GetPrivateRackForPlayer(playerIx);
			
			// validate that tileId is on that rack
			if (_tileTracker.GetTileLoc(tileId) != privateRack)
			{
				throw new UnityException("TurnManagerServer: Joker swap tile isn't on playerIx's private rack.");
			}
			
			// now do the swap
			_tileTracker.MoveTile(tileId, displayRack, jokerIx);
			_tileTracker.MoveTile(jokerTileId, privateRack);
			
			_tileTracker.SendGameStateToAll();
		}
		
		private bool CheckForMahJongg(int playerIx)
		{
			List<int> rack = _tileTracker.GetPrivateRackContentsForPlayer(playerIx);
			rack.AddRange(_tileTracker.GetLocContents(_tileTracker.GetDisplayRackForPlayer(playerIx)));
			if (!HandChecker.AnyMahJongg(rack, _card)) return false;
			_tileTracker.MakeRacksPublic();
			_fusionManager.MahJonggWinner = playerIx;
			_tileTracker.SendGameStateToAll();
			return true;
		}
	}
}