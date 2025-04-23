using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public class SetupServer
	{
		private TileTrackerServer _tileTracker;
		
		public void StartGame(FusionManagerServer fusionManager
			, out TileTrackerServer tileTracker, out TurnManagerServer turnManager)
		{
			// this will be a duplicate for the host but is good for if/when server is separated out
			List<Tile> tiles = new TileGenerator().GenerateTiles(); 
			tileTracker = new(tiles, fusionManager);
			_tileTracker = tileTracker;
			turnManager = new(tileTracker, fusionManager);

			Shuffle();
			Deal();
		}

		private void Shuffle()
		{
			// first shuffle the tiles
			List<int> shuffleTileList = Enumerable.Range(0, 152).ToList();
			System.Random rnd = new();
			for (int i = shuffleTileList.Count - 1; i > 0; i--)
			{
				int k = rnd.Next(i);
				(shuffleTileList[k], shuffleTileList[i]) = (shuffleTileList[i], shuffleTileList[k]);
			}
			
			// put them on the wall
			foreach (int tileId in shuffleTileList)
			{
				_tileTracker.MoveTile(tileId, SLoc.Wall);
			}
		}

		private void Deal(int dealerId = 0)
		{
			// 13 tiles to each player
			for (int playerId = 0; playerId < 4; playerId++)
			{
				for (int i = 0; i < 13; i++)
				{
					_tileTracker.PickupTileWallToRack(playerId);
				}
			}
			
			// 1 more tile to the dealer
			_tileTracker.PickupTileWallToRack(dealerId);
		}
	}
}
