using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Resources.ClientTests
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
		public void MoveTile_RackRearrange_TileMoves()
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
		
		[Test]
		public void RequestMoveTile_WhenCalled_GameStateDoesntChange()
		{
			_tileTracker.RequestMove(84, CLoc.Discard);
			
			Assert.AreEqual(CLoc.Pool, _tileTracker.GetTileLoc(84));
		}

		[Test]
		public void ReceiveGameState_NoPendingMove_UpdatesGameState()
		{
			CLoc[] newGameState = NewGameState();
			newGameState[10] = CLoc.LocalPrivateRack;
			newGameState[20] = CLoc.LocalDisplayRack;
			newGameState[30] = CLoc.OtherDisplayRack1;
			newGameState[40] = CLoc.Discard;
			
			_tileTracker.ReceiveGameState(101, newGameState);
			
			CollectionAssert.AreEqual(newGameState, _tileTracker.GameState);
		}

		[Test]
		public void ReceiveGameState_RequestIdMatchesPendingMove_PendingMoveConfirmed_GameStateUpdates()
		{
			_tileTracker.RequestMove(84, CLoc.LocalPrivateRack); // request id should be 10 but might change with later functionality
			CLoc[] newGameState = NewGameState();
			newGameState[84] = CLoc.LocalPrivateRack;
			
			_tileTracker.ReceiveGameState(10, newGameState);
			
			CollectionAssert.AreEqual(newGameState, _tileTracker.GameState);
		}
		
		[Test]
		public void ReceiveGameState_RequestIdMatchesPendingMove_PendingMoveNotConfirmed_ShowError()
		{
			_tileTracker.RequestMove(84, CLoc.LocalPrivateRack);
			CLoc[] newGameState = NewGameState();

			throw new NotImplementedException();
			// TODO: figure out how to actually handle this scenario
		}

	

		[Test]
		public void ReceiveGameState_RequestIdDoesntMatchPendingMove_UpdatesGameState()
		{
			_tileTracker.RequestMove(84, CLoc.LocalPrivateRack);
			CLoc[] newGameState = NewGameState();
			
			_tileTracker.ReceiveGameState(101, newGameState);
			
			CollectionAssert.AreEqual(newGameState, _tileTracker.GameState);
		}

		CLoc[] NewGameState()
		{
			CLoc[] newGameState = new CLoc[152];
			for (int tileId = 0; tileId < 152; tileId++)
			{
				newGameState[tileId] = CLoc.Pool;
			}
			
			return newGameState;
		} 
	}
}