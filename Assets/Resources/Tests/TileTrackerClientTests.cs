using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace Resources
{
	public class TileTrackerClientTests
	{
		[SetUp]
		public void Setup()
		{
			new TileGenerator().GenerateTiles();
		}
		
		[Test]
		public void GetTileLoc_WhenCalled_ReturnsTileLocation()
		{
			TileTrackerClient tileTracker = new TileTrackerClient();

			CLoc loc = tileTracker.GetTileLoc(132);
			
			Assert.AreEqual(CLoc.Pool, loc);
		}

		[Test]
		public void GetTileIx_WhenCalled_ReturnsTileIx()
		{
			TileTrackerClient tileTracker = new TileTrackerClient();
			
			int ix = tileTracker.GetTileIx(132);
			
			Assert.AreEqual(132, ix);
		}

		[Test]
		public void GetLocContents_WhenCalled_ReturnsListOfTiles()
		{
			TileTrackerClient tileTracker = new TileTrackerClient();
			
			List<int> contents = tileTracker.GetLocContents(CLoc.Pool);

			CollectionAssert.AreEqual(Enumerable.Range(0, 152), contents);
		}

		[Test]
		public void MoveTile_WhenCalledWithoutIx_MovesTile()
		{
			TileTrackerClient tileTracker = new TileTrackerClient();
			
			tileTracker.MoveTile(132, CLoc.LocalPrivateRack);
			
			Assert.AreEqual(CLoc.LocalPrivateRack, tileTracker.GetTileLoc(132));
		}

		[Test]
		public void MoveTile_WhenCalledWithIx_MovesTile()
		{
			TileTrackerClient tileTracker = new TileTrackerClient();
			
			tileTracker.MoveTile(132, CLoc.LocalPrivateRack);
			tileTracker.MoveTile(133, CLoc.LocalPrivateRack);
			tileTracker.MoveTile(134, CLoc.LocalPrivateRack, 1);
			
			CollectionAssert.AreEqual(new List<int> { 132, 134, 133 }
				, tileTracker.GetLocContents(CLoc.LocalPrivateRack));
		}
	}
}