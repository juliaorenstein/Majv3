using System.Collections.Generic;
using System.Diagnostics;

namespace Resources
{
	public class TileTrackerClient
	{
		public readonly IReadOnlyList<Tile> AllTiles;
		
		private readonly CLoc[] _gameState = new CLoc[152]; // location at index n is tile n's location
		public CLoc[] GameState => (CLoc[])_gameState.Clone();

		private readonly IMono _mono;
		
		private readonly Dictionary<CLoc, List<int>> _locToList = new();
		private int _nextRequestId;
		private PendingMove? _pendingMove;

		public TileTrackerClient(IMono mono, List<Tile> allTiles)
		{
			_mono = mono;
			AllTiles = allTiles;
			// TODO: set _nextRequestId to playerId to start.
			InitializeLocToList();
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				_gameState[tileId] = CLoc.Pool;
				_locToList[CLoc.Pool].Add(tileId);
			}
		}

		void InitializeLocToList()
		{
			_locToList[CLoc.LocalPrivateRack] = new();
			_locToList[CLoc.LocalDisplayRack] = new();
			_locToList[CLoc.OtherPrivateRack1] = new();
			_locToList[CLoc.OtherDisplayRack1] = new();
			_locToList[CLoc.OtherPrivateRack2] = new();
			_locToList[CLoc.OtherDisplayRack2] = new();
			_locToList[CLoc.OtherPrivateRack3] = new();
			_locToList[CLoc.OtherDisplayRack3] = new();
			_locToList[CLoc.Discard] = new();
			_locToList[CLoc.Pool] = new();
		}
		
		public CLoc GetTileLoc(int tileId) => _gameState[tileId];
		public int GetTileIx(int tileId) => _locToList[_gameState[tileId]].IndexOf(tileId);

		// Allow external callers to see contents of list without modifying
		public List<int> GetLocContents(CLoc loc) => new(_locToList[loc]);
		
		public void MoveTile(int tileId, CLoc newLoc, int ix = -1)
		{
			// if tile is already here, quit out
			if (_gameState[tileId] == newLoc) return;
			
			// remove tile from current location, add to new location
			CLoc currLoc = _gameState[tileId];
			_locToList[currLoc].Remove(tileId);
			
			// if ix is given, use it. Otherwise append to end of list
			if (ix == -1) _locToList[newLoc].Add(tileId);
			else _locToList[newLoc].Insert(ix, tileId);
			
			// update tile location
			_gameState[tileId] = newLoc;
			
			// update UI
			_mono.MoveTile(tileId, newLoc, ix);
		}

		public void ReceiveGameState(int requestId, CLoc[] newGameState)
		{
			// TODO: server needs to communicate number of tiles on other players' racks
			// this is a response to a requested move from this client. Verify pending move was accepted
			if (_pendingMove.HasValue && requestId == _pendingMove.Value.RequestId)
			{
				Debug.Assert(newGameState[_pendingMove.Value.TileId] == _pendingMove.Value.NewLoc);
				_pendingMove = null;
			}
			
			// apply all changes
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				// log when tiles are moving based on a game state update
				Debug.WriteLineIf(newGameState[tileId] != _gameState[tileId]
					, $"ReceiveGameState: {AllTiles[tileId]} ({tileId}) moving from {_gameState[tileId]} to {newGameState[tileId]}");
				MoveTile(tileId, newGameState[tileId]);
			}
		}

		public void RequestMove(int tileId, CLoc loc)
		{
			_pendingMove = new()
			{
				RequestId = _nextRequestId,
				TileId = tileId,
				OldLoc = _gameState[tileId],
				NewLoc = loc
			};
			_nextRequestId += 10;
			_mono.MoveTile(tileId, loc);
		}
		
		struct PendingMove
		{
			public int RequestId;
			public int TileId;
			public CLoc OldLoc;
			public CLoc NewLoc;
		}
	}
}