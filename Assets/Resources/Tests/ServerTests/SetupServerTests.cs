using System.Collections.Generic;
using NUnit.Framework;
using Resources;

namespace Resources.ServerTests
{
	public class SetupServerTests
	{
		// TODO: come back to this later
		/*
		[Test]
		public void StartGame_WhenCalled_AllTilesPopulatedInOrder()
		{
			List<Tile> tiles = new TileGenerator().GenerateTiles();
			SetupServer setupServer = new();
			setupServer.StartGame(tiles);
			List<Tile> allTilesInTileTracker = setupServer.tileTracker.AllTiles;

			CollectionAssert.AreEqual(tiles, allTilesInTileTracker);
		}

		[Test]
		public void StartGame_WhenCalled_DealsTilesToRacks()
		{
			List<Tile> tiles = new TileGenerator().GenerateTiles();
			SetupServer setupServer = new();
			setupServer.StartGame(tiles);
			TileTrackerServer tileTracker = setupServer.TileTracker;

			int rack0Count = tileTracker.GetLocContents(SLoc.PrivateRack0).Count;
			int rack1Count = tileTracker.GetLocContents(SLoc.PrivateRack1).Count;
			int rack2Count = tileTracker.GetLocContents(SLoc.PrivateRack2).Count;
			int rack3Count = tileTracker.GetLocContents(SLoc.PrivateRack3).Count;
			
			Assert.AreEqual(14, rack0Count);
			Assert.AreEqual(13, rack1Count);
			Assert.AreEqual(13, rack2Count);
			Assert.AreEqual(13, rack3Count);
		}
		*/
	}
}