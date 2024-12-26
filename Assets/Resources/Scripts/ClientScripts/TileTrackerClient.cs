using System.Collections.Generic;
using System.Diagnostics;

namespace Resources
{
	public class TileTrackerClient
	{
		public readonly IReadOnlyList<Tile> AllTiles;
		
		private readonly IMono _mono;
		private readonly NetworkedGameState _networkedGameState;

		private readonly CLoc[] _currentGameState;
		public CLoc[] GameStateFromServer => _networkedGameState.ClientGameState;
		private readonly int[] _privateRackCounts;
		private int[] PrivateRackCountsFromServer => _networkedGameState.PrivateRackCounts;
		
		private List<CLoc> PrivateRacks { get; } = new() 
			{ CLoc.LocalPrivateRack, CLoc.OtherPrivateRack1, CLoc.OtherPrivateRack2, CLoc.OtherPrivateRack3 };
		private List<CLoc> DisplayRacks { get; } = new()
			{ CLoc.LocalDisplayRack, CLoc.OtherDisplayRack1, CLoc.OtherDisplayRack2, CLoc.OtherDisplayRack3 };
		private CLoc GetPrivateRackForPlayer(int playerId) => PrivateRacks[( 4 + playerId - _networkedGameState.PlayerId) % 4];
		private CLoc GetDisplayRackForPlayer(int playerId) => DisplayRacks[( 4 + playerId - _networkedGameState.PlayerId) % 4];
		
		private readonly Dictionary<CLoc, List<int>> _inverseGameState = new();
		private int _nextRequestId;

		public TileTrackerClient(IMono mono, List<Tile> allTiles, FusionManagerClient fusionManagerClient)
		{
			// initialize variables
			_mono = mono;
			AllTiles = allTiles;
			_networkedGameState = fusionManagerClient.GameState;
			_currentGameState = new CLoc[AllTiles.Count];
			_privateRackCounts = new int[4];
				
			// TODO: set _nextRequestId to playerId to start.
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
		
		/* Moves tile on the backend. Should not be called directly from client request unless for a Rack Rearrange. All other tile moves should go through RequestMove */
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
			
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				// if tile is already here, quit out
				if (GameStateFromServer[tileId] == _currentGameState[tileId]) continue;
				MoveTile(tileId, GameStateFromServer[tileId]);
			}
			
			// update the tiles that display as private on other players' racks
			// start for loop at 1 to skip player's own rack
			for (int playerId = 0; playerId < 4; playerId++)
			{
				// skip this process for local player
				if (_networkedGameState.PlayerId == playerId) continue;
				
				CLoc privateRack = GetPrivateRackForPlayer(playerId);
				// if count already matches, continue
				if (PrivateRackCountsFromServer[playerId] == _privateRackCounts[playerId]) continue;
				// update the count in the privateRackCounts variable and on the UI
				_privateRackCounts[playerId] = PrivateRackCountsFromServer[playerId];
				_mono.UpdatePrivateRackCount(privateRack, PrivateRackCountsFromServer[playerId]);
			}
		}

		// Send a request for a move to the server. Also updates the UI for the player in the meantime.
		public void RequestMove(int tileId, CLoc loc)
		{
			_mono.MoveTile(tileId, loc);
		}
		
		// A struct to track requested moves that haven't been confirmed by the server
		struct PendingMove
		{
			public int RequestId;
			public int TileId;
			public CLoc OldLoc;
			public CLoc NewLoc;
		}
	}
}