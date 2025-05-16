using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public class SetupServer
	{
		private TileTrackerServer _tileTracker;
		private FusionManagerGlobal _fusionManager;
		private bool _devRacks = true;
		
		public void SetUp(FusionManagerGlobal fusionManager, CallHandler callHandler)
		{
			_fusionManager = fusionManager;
			List<Tile> tiles = new TileGenerator().GenerateTiles(); 
			_tileTracker = new(tiles, fusionManager);
			TurnManagerServer turnManager = new(_tileTracker, fusionManager, callHandler);
			fusionManager.TurnManagerServer = turnManager;
			CharlestonHandlerNetwork charlestonHandlerNetwork = fusionManager.GetComponent<CharlestonHandlerNetwork>();
			callHandler.TurnManager = turnManager;
			CharlestonHandlerServer charlestonHandler = new(_tileTracker, charlestonHandlerNetwork, fusionManager);
			foreach (InputReceiver receiver in fusionManager.GetComponentsInChildren<InputReceiver>())
			{
				receiver.Initialize(turnManager, callHandler, charlestonHandler, fusionManager); // Pass the shared instance
			}
			
			if (_devRacks) DontShuffle();
			else Shuffle();
			Deal();
		}

		private void Shuffle()
		{
			if (!_devRacks) {}
			// first shuffle the tiles
			List<int> shuffleTileList = Enumerable.Range(0, 152).ToList();
			System.Random rnd = new();
			for (int i = shuffleTileList.Count - 1; i > 0; i--)
			{
				int k = rnd.Next(i);
				(shuffleTileList[k], shuffleTileList[i]) = (shuffleTileList[i], shuffleTileList[k]);
			}
			
			// put them on the wall
			_tileTracker.PopulateWall(shuffleTileList);
		}

		private void DontShuffle()
		{
			List<int> tileList = Enumerable.Range(0, 152).ToList();
			_tileTracker.PopulateWall(tileList);
		}

		private void Deal(int dealerId = 1)
		{
			// 13 tiles to each player
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				for (int i = 0; i < 13; i++)
				{
					_tileTracker.PickupTileWallToRack(playerIx);
				}
			}
			
			// 1 more tile to the dealer
			_tileTracker.PickupTileWallToRack(dealerId);
		}
	}
}
