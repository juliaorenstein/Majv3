using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public class SetupServer
	{
		public TileTrackerServer TileTracker;
		
		public void StartGame(List<Tile> tiles)
		{
			TileTracker = new(tiles);
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
				TileTracker.AddTileToWall(tileId);
			}
		}

		private void Deal(int dealerId = 0)
		{
			// 13 tiles to each player
			for (int playerId = 0; playerId < 4; playerId++)
			{
				for (int i = 0; i < 13; i++)
				{
					TileTracker.MoveTileWallToRack(playerId);
				}
			}
			
			// 1 more tile to the dealer
			TileTracker.MoveTileWallToRack(dealerId);
		}
	}
}
