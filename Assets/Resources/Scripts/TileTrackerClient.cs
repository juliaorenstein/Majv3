using System.Collections.Generic;

namespace Resources
{
	public class TileTrackerClient
	{
		private Dictionary<int, CLoc> _tileToLoc = new();
		private Dictionary<CLoc, List<int>> _locToList = new();

		public TileTrackerClient()
		{
			InitializeLocToList();
			for (int tileId = 0; tileId < Tile.AllTiles.Count; tileId++)
			{
				_tileToLoc[tileId] = CLoc.Pool;
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
		
		public CLoc GetTileLoc(int tileId) => _tileToLoc[tileId];
		public int GetTileIx(int tileId) => _locToList[_tileToLoc[tileId]].IndexOf(tileId);

		// Allow external callers to see contents of list without modifying
		public List<int> GetLocContents(CLoc loc) => new(_locToList[loc]);
		
		public void MoveTile(int tileId, CLoc newLoc, int ix = -1)
		{
			// if tile is already here, quit out
			if (_tileToLoc[tileId] == newLoc) return;
			
			// remove tile from current location, add to new location
			CLoc currLoc = _tileToLoc[tileId];
			_locToList[currLoc].Remove(tileId);
			
			// if ix is given, use it. Otherwise append to end of list
			if (ix == -1) _locToList[newLoc].Add(tileId);
			else _locToList[newLoc].Insert(ix, tileId);
			
			// update tile location
			_tileToLoc[tileId] = newLoc;
			
			//
		}
	}
}