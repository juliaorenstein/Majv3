using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public class TileTrackerServer
	{
		private List<Tile> AllTiles { get; }
		private static List<SLoc> PrivateRacks { get; } = new()
			{ SLoc.PrivateRack0, SLoc.PrivateRack1, SLoc.PrivateRack2, SLoc.PrivateRack3 };
		private static List<SLoc> DisplayRacks { get; } = new()
			{ SLoc.DisplayRack0, SLoc.DisplayRack1, SLoc.DisplayRack2, SLoc.DisplayRack3 };
		private readonly Dictionary<SLoc, List<int>> _locToList = new();
		public SLoc GetPrivateRackForPlayer(int playerIx) => PrivateRacks[playerIx];
		public SLoc GetDisplayRackForPlayer(int playerIx) => DisplayRacks[playerIx];
		
		// Game States
		private readonly SLoc[] _serverGameState = new SLoc[152]; // location at index n is tile n's location
		public SLoc[] GameState => (SLoc[])_serverGameState.Clone();
		private int _gameStateVersion;
		
		private readonly IFusionManagerGlobal _fusionManager;

		public TileTrackerServer(List<Tile> tiles, IFusionManagerGlobal fusionManager)
		{
			InitializeLocToList();
			AllTiles = tiles;
			//_rpcS2CHandler = rpcS2CHandler;
			_fusionManager = fusionManager;
		}

		void InitializeLocToList()
		{
			_locToList[SLoc.PrivateRack0] = new();
			_locToList[SLoc.DisplayRack0] = new();
			_locToList[SLoc.PrivateRack1] = new();
			_locToList[SLoc.DisplayRack1] = new();
			_locToList[SLoc.PrivateRack2] = new();
			_locToList[SLoc.DisplayRack2] = new();
			_locToList[SLoc.PrivateRack3] = new();
			_locToList[SLoc.DisplayRack3] = new();
			_locToList[SLoc.Discard] = new();
			_locToList[SLoc.Wall] = new();
		}
		
		// Get the location (SLoc) of a tile
		public SLoc GetTileLoc(int tileId) => GameState[tileId];
		
		// Allow external callers to see contents of list without modifying
		public List<int> GetLocContents(SLoc loc) => new(_locToList[loc]);
		
		public List<int> GetPrivateRackContentsForPlayer(int playerIx) => 
			GetLocContents(GetPrivateRackForPlayer(playerIx));
		public List<int> GetDisplayRackContentsForPlayer(int playerIx) => 
			GetLocContents(GetDisplayRackForPlayer(playerIx));

		public bool PlayerPrivateRackContains(int playerIx, int tileId) =>
			GetPrivateRackContentsForPlayer(playerIx).Contains(tileId);
		public bool PlayerDisplayRackContains(int playerIx, int tileId) => 
			GetDisplayRackContentsForPlayer(playerIx).Contains(tileId);
		
		public void PopulateWall(List<int> tileIds)
		{
			foreach (int tileId in tileIds)
			{
				_locToList[SLoc.Wall].Add(tileId);
				_serverGameState[tileId] = SLoc.Wall; // already true by default but just in case
			}
			SendGameStateToAll();
		}
		
		// GENERIC MOVE TILE
		public void MoveTile(int tileId, SLoc newLoc, int ix = -1, bool sendToAll = true)
		{
			// if tile is already here, quit out
			if (_serverGameState[tileId] == newLoc) return;
			
			// remove tile from current location, add to new location
			SLoc currLoc = _serverGameState[tileId];
			_locToList[currLoc].Remove(tileId);
			
			// if ix is given, use it. Otherwise append to end of list
			if (ix == -1) _locToList[newLoc].Add(tileId);
			else _locToList[newLoc].Insert(ix, tileId);
			
			// update tile location
			_serverGameState[tileId] = newLoc;
			
			// option not to send to all if batch updating (like dealing and charleston moves)
			// TODO: batch update for dealing
			if (sendToAll) SendGameStateToAll();
		}

		// SPECIALTY METHODS
		public void PickupTileWallToRack(int playerIx)
		{
			SLoc rack = PrivateRacks[playerIx];
			int tileId = GetLocContents(SLoc.Wall).Last();
			MoveTile(tileId, rack);
		}

		// CLIENT COMMUNICATION
		public void SendGameStateToAll(bool updateUI = true)
		{
			_gameStateVersion = Math.Abs(_gameStateVersion) + 1;
			if (!updateUI) _gameStateVersion = -_gameStateVersion; // client will not update UI if version < 0
			
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				SendGameStateToPlayer(playerIx);
			}
		}

		// TODO: implement invalidNotif - tell players if this game state indicates a request was rejected
		public void SendGameStateToPlayer(int playerIx, bool invalidNotif = false)
		{
			CLoc[] clientGameState = new CLoc[AllTiles.Count];
			Dictionary<SLoc, CLoc> sLocToCLoc = SLocToCLoc(playerIx);
			// translates each entry in tileToLoc to an entry for the client
			// (for ex, tiles on other player's racks show as in the pool)
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				SLoc sLoc = GameState[tileId];
				clientGameState[tileId] = sLocToCLoc[sLoc];
			}

			INetworkedGameState networkedGameState = _fusionManager.NetworkedGameStates[playerIx];
			networkedGameState.GameStateVersion = _gameStateVersion;
			networkedGameState.UpdateClientGameState(clientGameState);

			// update private rack counts
			int[] privateRackCounts = new int[4];
			for (int rackId = 0; rackId < 4; rackId++)
			{
				privateRackCounts[rackId] = _locToList[PrivateRacks[rackId]].Count;
			}
			networkedGameState.UpdatePrivateRackCounts(privateRackCounts);
		}

		// UTILITY
		Dictionary<SLoc, CLoc> SLocToCLoc(int playerIx)
		{
			Dictionary<SLoc, CLoc> ret = new()
			{
				// private racks
				[PrivateRacks[playerIx]] = CLoc.LocalPrivateRack,
				[PrivateRacks[(playerIx + 1) % 4]] = CLoc.Pool,
				[PrivateRacks[(playerIx + 2) % 4]] = CLoc.Pool,
				[PrivateRacks[(playerIx + 3) % 4]] = CLoc.Pool,
				// display racks
				[DisplayRacks[playerIx]] = CLoc.LocalDisplayRack,
				[DisplayRacks[(playerIx + 1) % 4]] = CLoc.OtherDisplayRack1,
				[DisplayRacks[(playerIx + 2) % 4]] = CLoc.OtherDisplayRack2,
				[DisplayRacks[(playerIx + 3) % 4]] = CLoc.OtherDisplayRack3,
				// other
				[SLoc.Discard] = CLoc.Discard,
				[SLoc.Wall] = CLoc.Pool
			};

			return ret;
		}
	}
}