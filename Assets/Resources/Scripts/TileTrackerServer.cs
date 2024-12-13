using System.Collections.Generic;
using System.Diagnostics;
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
		
		private readonly Dictionary<int, SLoc> _tileToLoc = new();
		private readonly Dictionary<SLoc, List<int>> _locToList = new();

		public TileTrackerServer(List<Tile> tiles)
		{
			InitializeLocToList();
			AllTiles = tiles;
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
		
		public SLoc GetTileLoc(int tileId) => _tileToLoc[tileId];
		public int GetTileIx(int tileId) => _locToList[_tileToLoc[tileId]].IndexOf(tileId);

		// Allow external callers to see contents of list without modifying
		public List<int> GetLocContents(SLoc loc) => new(_locToList[loc]);
		
		public void MoveTile(int tileId, SLoc newLoc, int ix = -1)
		{
			// if tile is already here, quit out
			if (_tileToLoc[tileId] == newLoc) return;
			
			// remove tile from current location, add to new location
			SLoc currLoc = _tileToLoc[tileId];
			_locToList[currLoc].Remove(tileId);
			
			// if ix is given, use it. Otherwise append to end of list
			if (ix == -1) _locToList[newLoc].Add(tileId);
			else _locToList[newLoc].Insert(ix, tileId);
			
			// update tile location
			_tileToLoc[tileId] = newLoc;
		}

		public void AddTileToWall(int tileId)
		{
			_tileToLoc[tileId] = SLoc.Wall;
			_locToList[SLoc.Wall].Add(tileId);
		}

		public void MoveTileWallToRack(int playerId)
		{
			SLoc rack = PrivateRacks[playerId];
			int tileId = GetLocContents(SLoc.Wall).Last();
			MoveTile(tileId, rack);
		}

		public void SendGameState(Dictionary<int, SLoc> gameState)
		{
			
		}
	}
}