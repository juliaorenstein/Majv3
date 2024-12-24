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
		private readonly SLoc[] _gameState = new SLoc[152]; // location at index n is tile n's location
		public SLoc[] GameState => (SLoc[])_gameState.Clone();
		private readonly Dictionary<SLoc, List<int>> _locToList = new();

		private readonly IRpcS2CHandler _rpcS2CHandler;
		private readonly IFusionManagerServer _fusionManager;

		public TileTrackerServer(List<Tile> tiles, IRpcS2CHandler rpcS2CHandler, IFusionManagerServer fusionManager)
		{
			InitializeLocToList();
			AllTiles = tiles;
			_rpcS2CHandler = rpcS2CHandler;
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
			SLoc currLoc = _gameState[tileId];
			_locToList[currLoc].Remove(tileId);
			
			// if ix is given, use it. Otherwise append to end of list
			if (ix == -1) _locToList[newLoc].Add(tileId);
			else _locToList[newLoc].Insert(ix, tileId);
			
			// update tile location
			_gameState[tileId] = newLoc;
			
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
			for (int playerId = 0; playerId < _fusionManager.PlayerCount; playerId++)
			{
				SendGameStateToPlayer(playerId);
			}
		}

		public void SendGameStateToPlayer(int playerId, int requestId = -1)
		{
			CLoc[] playerGameState = new CLoc[AllTiles.Count];
			Dictionary<SLoc, CLoc> sLocToCLoc = SLocToCLoc(playerId);
			
			// translates each entry in tileToLoc to an entry for the client
			// (for ex, tiles on other player's racks show as in the pool)
			for (int tileId = 0; tileId < AllTiles.Count; tileId++)
			{
				SLoc sLoc = GameState[tileId];
				playerGameState[tileId] = sLocToCLoc[sLoc];
			}

			_rpcS2CHandler.RPC_S2C_SendGameState(playerId, requestId, playerGameState);
		}
		
		Dictionary<SLoc, CLoc> SLocToCLoc(int playerId)
		{
			Dictionary<SLoc, CLoc> ret = new();
			
			// private racks
			ret[PrivateRacks[playerId]] = CLoc.LocalPrivateRack;
			ret[PrivateRacks[(playerId + 1) % 4]] = CLoc.Pool;
			ret[PrivateRacks[(playerId + 2) % 4]] = CLoc.Pool;
			ret[PrivateRacks[(playerId + 3) % 4]] = CLoc.Pool;
			// display racks
			ret[DisplayRacks[playerId]] = CLoc.LocalDisplayRack;
			ret[DisplayRacks[(playerId + 1) % 4]] = CLoc.OtherDisplayRack1;
			ret[DisplayRacks[(playerId + 2) % 4]] = CLoc.OtherDisplayRack2;
			ret[DisplayRacks[(playerId + 3) % 4]] = CLoc.OtherDisplayRack3;
			// other
			ret[SLoc.Discard] = CLoc.Discard;
			ret[SLoc.Wall] = CLoc.Pool;

			return ret;
		}
	}
}