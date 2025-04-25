using System.Collections.Generic;


namespace Resources
{
	public class TileTrackerClient
	{
		public readonly IReadOnlyList<Tile> AllTiles;
		
		private readonly IMono _mono;
		public INetworkedGameState GameState;
		private readonly InputSender _inputSender;
		private readonly IFusionManagerGlobal _fusionManager;

		private readonly CLoc[] _currentGameState;
		public CLoc[] GameStateFromServer => GameState.ClientGameState;
		private readonly int[] _privateRackCounts;
		private int[] PrivateRackCountsFromServer => GameState.PrivateRackCounts;
		
		private List<CLoc> PrivateRacks { get; } = new() 
			{ CLoc.LocalPrivateRack, CLoc.OtherPrivateRack1, CLoc.OtherPrivateRack2, CLoc.OtherPrivateRack3 };
		public List<CLoc> DisplayRacks { get; } = new()
			{ CLoc.LocalDisplayRack, CLoc.OtherDisplayRack1, CLoc.OtherDisplayRack2, CLoc.OtherDisplayRack3 };
		private CLoc GetPrivateRackForPlayer(int playerIx) => PrivateRacks[( 4 + playerIx - GameState.PlayerIx) % 4];
		private CLoc GetDisplayRackForPlayer(int playerIx) => DisplayRacks[( 4 + playerIx - GameState.PlayerIx) % 4];
			
		
		private readonly Dictionary<CLoc, List<int>> _inverseGameState = new();

		public TileTrackerClient(IMono mono, List<Tile> allTiles, InputSender inputSender, IFusionManagerGlobal fusionManager)
		{
			// initialize variables
			_mono = mono;
			AllTiles = allTiles;
			_inputSender = inputSender;
			_currentGameState = new CLoc[AllTiles.Count];
			_privateRackCounts = new int[4];
			_fusionManager = fusionManager;
			
			InitializeLocToList();
			// put all the tiles in the tile pool to start
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				_currentGameState[tileId] = CLoc.Pool;
				_inverseGameState[CLoc.Pool].Add(tileId);
			}
		}

		void InitializeLocToList()
		{
			_inverseGameState[CLoc.LocalPrivateRack] = new();
			_inverseGameState[CLoc.LocalDisplayRack] = new();
			_inverseGameState[CLoc.OtherPrivateRack1] = new();
			_inverseGameState[CLoc.OtherDisplayRack1] = new();
			_inverseGameState[CLoc.OtherPrivateRack2] = new();
			_inverseGameState[CLoc.OtherDisplayRack2] = new();
			_inverseGameState[CLoc.OtherPrivateRack3] = new();
			_inverseGameState[CLoc.OtherDisplayRack3] = new();
			_inverseGameState[CLoc.Discard] = new();
			_inverseGameState[CLoc.Pool] = new();
		}
		
		// returns tile's location (CLoc). If unknown, returns Pool.
		public CLoc GetTileLoc(int tileId) => GameStateFromServer[tileId];
		// returns position of tile in its location
		public int GetTileIx(int tileId) => _inverseGameState[GameStateFromServer[tileId]].IndexOf(tileId);
		// Allow external callers to see contents of list without modifying
		public List<int> GetLocContents(CLoc loc) => new(_inverseGameState[loc]);
		
		// Moves tile on backend and calls mono for front end update
		public void MoveTile(int tileId, CLoc newLoc, int ix = -1)
		{
			// remove tile from current location, add to new location
			CLoc currLoc = _currentGameState[tileId];
			_inverseGameState[currLoc].Remove(tileId);
			
			// if ix is given, use it. Otherwise append to end of list
			if (ix == -1) _inverseGameState[newLoc].Add(tileId);
			else _inverseGameState[newLoc].Insert(ix, tileId);
			
			// update tile location
			_currentGameState[tileId] = newLoc;
			
			// update UI
			_mono.MoveTile(tileId, newLoc, ix);
		}
		
		public void UpdateGameState()
		{
			// TODO: Right now racks at start of game are being sorted by tileId because this goes through tiles by id.
			// Clear input
			_inputSender.ClearInput();
			UpdateButtons();
			
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				// if tile is already here, quit out
				if (GameStateFromServer[tileId] == _currentGameState[tileId]) continue;
				MoveTile(tileId, GameStateFromServer[tileId]);
			}
			
			// update the tiles that display as private on other players' racks
			// start for loop at 1 to skip player's own rack
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				// skip this process for local player
				if (GameState.PlayerIx == playerIx) continue;
				
				CLoc privateRack = GetPrivateRackForPlayer(playerIx);
				// if count already matches, continue
				if (PrivateRackCountsFromServer[playerIx] == _privateRackCounts[playerIx]) continue;
				// update the count in the privateRackCounts variable and on the UI
				_privateRackCounts[playerIx] = PrivateRackCountsFromServer[playerIx];
				_mono.UpdatePrivateRackCount(privateRack, PrivateRackCountsFromServer[playerIx]);
			}

			return;

			void UpdateButtons()
			{
				// Pick Up - enable if it's my turn and I haven't picked up yet
				_mono.SetActionButton(Action.PickUp
					, _fusionManager.CurrentTurnStage == TurnStage.PickUp 
					  && _fusionManager.IsMyTurn);
				
				// Call, Wait, Pass - enable if somebody discarded
				bool state = _fusionManager.CurrentTurnStage == TurnStage.Call 
				             && !_fusionManager.IsMyTurn
				             && !Tile.IsJoker(_fusionManager.DiscardTileId)
				             && _fusionManager.CurrentTurnStage != TurnStage.PickUp;
				_mono.SetActionButton(Action.Call, state);
				_mono.SetActionButton(Action.Wait, state);
				_mono.SetActionButton(Action.Pass, state);
				
				// NeverMind - enable if I called expose but haven't put out any subsequent tiles
				_mono.SetActionButton(Action.NeverMind
					, _fusionManager.CurrentTurnStage == TurnStage.Expose 
					  && _fusionManager.IsMyExpose
					  && GetTileLoc(_fusionManager.DiscardTileId) is CLoc.LocalDisplayRack);
			}
		}

		// Send a request for a move to the server. Also updates the UI for the player in the meantime.
		// TODO: remove this?
		public void RequestMove(int tileId, CLoc loc)
		{
			_mono.MoveTile(tileId, loc);
		}
	}
}