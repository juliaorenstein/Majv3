using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public class TileTrackerServer
	{
		public List<Tile> AllTiles { get; private set; }
		public static List<SLoc> PrivateRacks { get; } = new() 
			{ SLoc.PrivateRack0, SLoc.PrivateRack1, SLoc.PrivateRack2, SLoc.PrivateRack3 };
		public static List<SLoc> DisplayRacks { get; } = new()
			{ SLoc.DisplayRack0, SLoc.DisplayRack1, SLoc.DisplayRack2, SLoc.DisplayRack3 };
		private readonly Dictionary<SLoc, List<int>> _locToList = new();
		
		// Game States
		private readonly SLoc[] _serverGameState = new SLoc[152]; // location at index n is tile n's location
		public SLoc[] GameState => (SLoc[])_serverGameState.Clone();

		private readonly IRpcS2CHandler _rpcS2CHandler;
		private readonly IFusionManagerServer _fusionManager;

		public TileTrackerServer(List<Tile> tiles, IFusionManagerServer fusionManager)
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
		
		public void MoveTile(int tileId, SLoc newLoc, int ix = -1)
		{
			// if tile is already here, quit out
			if (GameState[tileId] == newLoc) return;
			
			// remove tile from current location, add to new location
			SLoc currLoc = _serverGameState[tileId];
			_locToList[currLoc].Remove(tileId);
			
			// if ix is given, use it. Otherwise append to end of list
			if (ix == -1) _locToList[newLoc].Add(tileId);
			else _locToList[newLoc].Insert(ix, tileId);
			
			// update tile location
			_serverGameState[tileId] = newLoc;
			
			SendGameStateToAll();
		}

		public void PickupTileWallToRack(int playerId)
		{
			SLoc rack = PrivateRacks[playerId];
			int tileId = GetLocContents(SLoc.Wall).Last();
			MoveTile(tileId, rack);
		}

		private void SendGameStateToAll()
		{
			for (int playerId = 0; playerId < 4; playerId++)
			{
				SendGameStateToPlayer(playerId);
			}
		}

		public void SendGameStateToPlayer(int playerId)
		{
			CLoc[] clientGameState = new CLoc[AllTiles.Count];
			Dictionary<SLoc, CLoc> sLocToCLoc = SLocToCLoc(playerId);

			// translates each entry in tileToLoc to an entry for the client
			// (for ex, tiles on other player's racks show as in the pool)
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				SLoc sLoc = GameState[tileId];
				clientGameState[tileId] = sLocToCLoc[sLoc];
			}

			NetworkedGameState networkedGameState = _fusionManager.NetworkedGameStates[playerId];
			networkedGameState.UpdateClientGameState(clientGameState);

			// update private rack counts
			int[] privateRackCounts = new int[4];
			for (int rackId = 0; rackId < 4; rackId++)
			{
				privateRackCounts[rackId] = _locToList[PrivateRacks[rackId]].Count;
			}
			networkedGameState.UpdatePrivateRackCounts(privateRackCounts);
		}

		Dictionary<SLoc, CLoc> SLocToCLoc(int playerId)
		{
			Dictionary<SLoc, CLoc> ret = new()
			{
				// private racks
				[PrivateRacks[playerId]] = CLoc.LocalPrivateRack,
				[PrivateRacks[(playerId + 1) % 4]] = CLoc.Pool,
				[PrivateRacks[(playerId + 2) % 4]] = CLoc.Pool,
				[PrivateRacks[(playerId + 3) % 4]] = CLoc.Pool,
				// display racks
				[DisplayRacks[playerId]] = CLoc.LocalDisplayRack,
				[DisplayRacks[(playerId + 1) % 4]] = CLoc.OtherDisplayRack1,
				[DisplayRacks[(playerId + 2) % 4]] = CLoc.OtherDisplayRack2,
				[DisplayRacks[(playerId + 3) % 4]] = CLoc.OtherDisplayRack3,
				// other
				[SLoc.Discard] = CLoc.Discard,
				[SLoc.Wall] = CLoc.Pool
			};

			return ret;
		}
	}
}