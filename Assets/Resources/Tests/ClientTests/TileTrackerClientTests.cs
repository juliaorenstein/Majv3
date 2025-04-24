using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Resources.ClientTests
{
	public class TileTrackerClientTests
	{
		private TileTrackerClient _tileTracker;
		private FakeNetworkedGameState _fakeNetworkedGameState;
		private IMono _mono;
		
		[SetUp]
		public void Setup()
		{
			_mono = new FakeMono();
			List<Tile> tiles = new TileGenerator().GenerateTiles();
			_fakeNetworkedGameState = new FakeNetworkedGameState();
			_tileTracker = new(_mono, tiles, new(), new FakeFusionManagerGlobal());
			_tileTracker.GameState = _fakeNetworkedGameState;
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
		public void MoveTile_RackRearrange_TileMoves()
		{ 
			_tileTracker.MoveTile(13, CLoc.LocalPrivateRack);
			
			Assert.AreEqual(CLoc.LocalPrivateRack, _tileTracker.GetTileLoc(13));
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
		
		[Test]
		public void RequestMoveTile_WhenCalled_GameStateDoesntChange()
		{
			_tileTracker.RequestMove(84, CLoc.Discard);
			
			Assert.AreEqual(CLoc.Pool, _tileTracker.GetTileLoc(84));
		}

		[Test]
		public void ReceiveGameState_UpdatesGameState()
		{
			CLoc[] newGameState = NewGameState();
			newGameState[10] = CLoc.LocalPrivateRack;
			newGameState[20] = CLoc.LocalDisplayRack;
			newGameState[30] = CLoc.OtherDisplayRack1;
			newGameState[40] = CLoc.Discard;
			
			_fakeNetworkedGameState.ClientGameState = newGameState;
			_tileTracker.UpdateGameState();
			
			CollectionAssert.AreEqual(newGameState, _tileTracker.GameStateFromServer);
		}

		CLoc[] NewGameState()
		{
			CLoc[] newGameState = new CLoc[152];
			for (int tileId = 0; tileId < 152; tileId++)
			{
				newGameState[tileId] = CLoc.Pool;
			}
			
			// deal out private rack
			for (int tileId = 10; tileId < 44; tileId++)
			{
				newGameState[tileId] = CLoc.LocalPrivateRack;
			}
			
			return newGameState;
		} 
	}
}