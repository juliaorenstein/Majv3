using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using Assert = NUnit.Framework.Assert;

namespace Resources.ServerTests
{
	public class TileTrackerServerTests
	{
		private TileTrackerServer _tileTracker;
		
		[SetUp]
		public void Setup()
		{
			List<Tile> tiles = new TileGenerator().GenerateTiles();
			_tileTracker = new(tiles, new FakeFusionManagerGlobal());
			// move all the tiles to the wall. This doesn't happen in TileTrackerServer constructor
			_tileTracker.PopulateWall(Enumerable.Range(0, 152).ToList());
		}
		
		[Test]
		public void GetTileLoc_AfterMove_ReturnTileLoc()
		{
			_tileTracker.MoveTile(84, SLoc.PrivateRack1);

			SLoc tileLoc = _tileTracker.GetTileLoc(84);
			
			Assert.AreEqual(SLoc.PrivateRack1, tileLoc);
		}

		[Test]
		public void GetLocContents_WhenCalled_ReturnsLocContents()
		{
			_tileTracker.MoveTile(84, SLoc.PrivateRack1);
			_tileTracker.MoveTile(86, SLoc.PrivateRack1);
			_tileTracker.MoveTile(85, SLoc.PrivateRack1);

			List<int> actualContents = _tileTracker.GetLocContents(SLoc.PrivateRack1);
			List<int> expectedContents = new() {84, 86, 85};
			
			CollectionAssert.AreEqual(expectedContents, actualContents);
		}
		
		[Test]
		public void MoveTile_TileAlreadyThere_NothingChanges()
		{
			SLoc[] gameStateBefore = _tileTracker.GameState;
			_tileTracker.MoveTile(84, SLoc.Wall);
			SLoc[] gameStateAfter = _tileTracker.GameState;
			
			CollectionAssert.AreEqual(gameStateBefore, gameStateAfter);
		}

		[Test]
		public void MoveTile_NewLocation_TileMoves()
		{
			_tileTracker.MoveTile(84, SLoc.PrivateRack1);
			
			SLoc tileLoc = _tileTracker.GetTileLoc(84);
			
			Assert.AreEqual(SLoc.PrivateRack1, tileLoc);
		}

		[Test]
		public void PickupTileWallToRack_WhenCalled_MovesTileToRack()
		{
			int tileId = 151; // should be last tile in unshuffled wall
			
			
			_tileTracker.PickupTileWallToRack(2);
			
			SLoc tileLoc = _tileTracker.GetTileLoc(tileId);
			
			List<int> expectedContents = new() { tileId };
			List<int> actualContents = _tileTracker.GetLocContents(SLoc.PrivateRack2);
			
			Assert.AreEqual(SLoc.PrivateRack2, tileLoc);
			Assert.AreEqual(expectedContents, actualContents);
		}
	}
}