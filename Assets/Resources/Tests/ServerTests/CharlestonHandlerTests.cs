using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace Resources.ServerTests
{
	public class CharlestonHandlerTests
	{
		private CharlestonHandlerServer _charlestonHandler;
		private TileTrackerServer _tileTracker;
		
		[SetUp]
		public void SetUp()
		{
			List<Tile> tiles = new TileGenerator().GenerateTiles();
			FakeFusionManagerGlobal fakeFusionManager = new();
			_tileTracker = new(tiles, fakeFusionManager);
			_charlestonHandler = new(_tileTracker, new CharlestonHandlerNetwork(), fakeFusionManager); // TODO: Abstract this out
			SetUpRacks();
		}

		// BASIC THREE TILE PASSES - SHOULD WORK
		[Test]
		public void DoPass_PassRightBasic_DoesPass()
		{
			InitializePass();
			
			int[][] expectedResArr =
			{
				new [] { 90, 91, 92 },
				new [] { 0, 1, 2 },
				new [] { 30, 31, 32 },
				new [] { 60, 61, 62 }
			};

			AssertResult(expectedResArr);
		}

		[Test]
		public void DoPass_PassAcrossBasic_DoesPass()
		{
			InitializePass(); // pass 1
			InitializePass(); // pass 2
			
			int[][] expectedResArr =
			{
				new [] { 63, 64, 65 },
				new [] { 93, 94, 95 },
				new [] { 3, 4, 5 },
				new [] { 33, 34, 35 }
			};
			
			AssertResult(expectedResArr);
		}
		
		[Test]
		public void DoPass_PassLeftBasic_DoesPass()
		{
			InitializePass(); // pass right
			InitializePass(); // pass across
			InitializePass(); // pass left
			
			int[][] expectedResArr =
			{
				new [] { 36, 37, 38 },
				new [] { 66, 67, 68 },
				new [] { 96, 97, 98 },
				new [] { 6, 7, 8 }
			};

			AssertResult(expectedResArr);
		}
		
		// VALID PARTIAL PASSES (FIRST LEFT, SECOND RIGHT)
		[Test]
		public void DoPass_SinglePlayer2TilePassOnFirstLeft_DoesPass()
		{
			InitializePass();
			InitializePass();
			InitializePass(player3Num: 2);
			
			int[][] expectedResArr =
			{
				new [] { 36, 37, 38 },
				new [] { 66, 67, 68 },
				new [] { 96, 97, 8 },
				new [] { 6, 7 }
			};
			
			AssertResult(expectedResArr);
		}

		[Test]
		public void DoPass_AllPlayer2TilePassOnFirstLeft_DoesPass()
		{
			InitializePass();
			InitializePass();
			InitializePass(2, 2, 2, 2);
			
			int[][] expectedResArr =
			{
				new [] { 36, 37 },
				new [] { 66, 67 },
				new [] { 96, 97 },
				new [] { 6, 7 }
			};
			
			AssertResult(expectedResArr);
		}
		
		[Test]
		public void DoPass_2TilePassOnSecondRight_DoesPass()
		{
			InitializePass(); // right 
			InitializePass(); // across
			InitializePass(); // left
			InitializePass(); // left 
			InitializePass(); // across
			InitializePass(player0Num: 3); // right
			
			int[][] expectedResArr =
			{
				new [] { 105, 106, 107 },
				new [] { 15, 16, 17 },
				new [] { 45, 46, 47 },
				new [] { 75, 76, 77 }
			};

			AssertResult(expectedResArr);
		}
		
		[Test]
		public void DoPass_AllPlayerDifferentNumsOnFirstLeft_DoesPass()
		{
			InitializePass(); // right
			InitializePass(); // across
			InitializePass(0, 3, 1, 2); // left
			
			int[][]	expectedResArr =
			{
				new int [0],
				new [] { 66, 97, 38 },
				new [] { 96 },
				new [] { 36, 37 }
			};
			
			AssertResult(expectedResArr);
		}

		[Test]
		public void DoPass_OptionalAcross_SameNumbersInPairs_DoesPass()
		{
			InitializePass(); // right
			InitializePass(); // across
			InitializePass(); // left
			InitializePass(); // left
			InitializePass(); // across
			InitializePass(); // right
			InitializePass(2, 1, 2, 1); // across
			
			int[][] expectedResArr =
			{
				new [] { 63, 64 },
				new [] { 93 },
				new [] { 3, 4 },
				new [] { 33 }
			};
			
			AssertResult(expectedResArr);
		}

		[Test]
		public void DoPass_OptionalAcross_DifferentNumbersInPairs_PromptRepick()
		{
			InitializePass(); // right
			InitializePass(2, 4, 3, 1); // across
			
			int[][] expectedResArr =
			{
				new [] { 63, 64 },
				new [] { 93 },
				new [] { 3, 4 },
				new [] { 33 }
			};

			Assert.Fail();
		}

		// INVALID INPUTS - THROW EXCEPTIONS
		[Test]
		public void DoPass_2TilePassOnFirstRight_ThrowsException()
		{
			TestDelegate shouldThrowException = () => InitializePass(player2Num: 2);
			
			Assert.Throws<UnityEngine.UnityException>(shouldThrowException);
		}

		
		[Test]
		public void PassLeft_2TilePassOnSecondLeft_ThrowsException()
		{
			InitializePass(); // right
			InitializePass(); // across
			InitializePass(); // left
			TestDelegate shouldThrowException = () => InitializePass(player2Num: 2); // left
			
			Assert.Throws<UnityEngine.UnityException>(shouldThrowException);
		}

		private void SetUpRacks()
		{ 
			List<int>[]privateRacks =
			{
				Enumerable.Range(0, 21).ToList(),
				Enumerable.Range(30, 21).ToList(),
				Enumerable.Range(60, 21).ToList(),
				Enumerable.Range(90, 21).ToList()
			};

			for (int playerIx = 0; playerIx < privateRacks.Length; playerIx++)
			{
				foreach (int tileId in privateRacks[playerIx])
				{
					_tileTracker.MoveTile(tileId, _tileTracker.GetPrivateRackForPlayer(playerIx));
				}
			}
		}

		private void InitializePass(int player0Num = 3, int player1Num = 3, int player2Num = 3, int player3Num = 3)
		{
			// TODO: need to rewrite this
		}

		private void AssertResult(int[][] expectedResArr)
		{
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				CollectionAssert.IsSubsetOf(expectedResArr[playerIx]
					, _tileTracker.GetPrivateRackContentsForPlayer(playerIx));
				Assert.True(_tileTracker.GetPrivateRackContentsForPlayer(playerIx).Count == 21);
			}
		}
	}
}