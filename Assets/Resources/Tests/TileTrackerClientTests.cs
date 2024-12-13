using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace Resources
{
	public class TileTrackerClientTests
	{
		private TileTrackerClient _tileTracker;
		private IMono _mono;
		
		[SetUp]
		public void Setup()
		{
			_mono = new FakeMono();
			List<Tile> tiles = new TileGenerator().GenerateTiles();
			_tileTracker = new(_mono, tiles);
		}
		
		[Test]
		public void GetTileLoc_WhenCalled_ReturnsTileLocation()
		{
			CLoc loc = _tileTracker.GetTileLoc(132);
			
			Assert.AreEqual(CLoc.Pool, loc);
		}

		[Test]
		public void GetTileIx_WhenCalled_ReturnsTileIx()
		{
			int ix = _tileTracker.GetTileIx(132);
			
			Assert.AreEqual(132, ix);
		}

		[Test]
		public void GetLocContents_WhenCalled_ReturnsListOfTiles()
		{
			List<int> contents = _tileTracker.GetLocContents(CLoc.Pool);

			CollectionAssert.AreEqual(Enumerable.Range(0, 152), contents);
		}

		[Test]
		public void MoveTile_WhenCalledWithoutIx_MovesTile()
		{ 
			_tileTracker.MoveTile(132, CLoc.LocalPrivateRack);
			
			Assert.AreEqual(CLoc.LocalPrivateRack, _tileTracker.GetTileLoc(132));
		}

		[Test]
		public void MoveTile_WhenCalledWithIx_MovesTile()
		{
			_tileTracker.MoveTile(132, CLoc.LocalPrivateRack);
			_tileTracker.MoveTile(133, CLoc.LocalPrivateRack);
			_tileTracker.MoveTile(134, CLoc.LocalPrivateRack, 1);
			
			CollectionAssert.AreEqual(new List<int> { 132, 134, 133 }
				, _tileTracker.GetLocContents(CLoc.LocalPrivateRack));
		}
	}
}