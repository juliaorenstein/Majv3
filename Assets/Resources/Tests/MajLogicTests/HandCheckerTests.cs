using System.Collections.Generic;
using NUnit.Framework;

namespace Resources.MajLogicTests
{
	public class HandCheckerTests
	{
		[Test]
		public void CheckHand_BasicConsec_ExactMatch()
		{
			string handString = "FFF 1111g FFF 2222r consec";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.Number, -1, 1, Suit.Bam),
				new(Kind.Number, -1, 1, Suit.Bam),
				new(Kind.Number, -1, 1, Suit.Bam),
				new(Kind.Number, -1, 1, Suit.Bam),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.Number, -1, 2, Suit.Crak),
				new(Kind.Number, -1, 2, Suit.Crak),
				new(Kind.Number, -1, 2, Suit.Crak),
				new(Kind.Number, -1, 2, Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_BasicConsec_Permutation()
		{
			string handString = "FFF 1111g FFF 2222r consec";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Number, -1, 3, Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_BasicConsec_WithJokers()
		{
			string handString = "FFF 1111g FFF 2222r consec";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.Joker),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Joker),
				new(Kind.Number, -1, 3, Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_BasicConsec_Invalid()
		{
			string handString = "FFF 1111g FFF 2222r consec";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.North),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.Number, -1, 4, Suit.Dot),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.FlowerWind, -1, 0, Suit.None, Wind.Flower),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Number, -1, 3, Suit.Crak),
				new(Kind.Number, -1, 3, Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsFalse(result);
		}

		[Test]
		public void CheckHand_Quints_ExactMatch()
		{
			string handString = "DDDDDb NNNN 11111b consec";
			List<Tile> tiles = new()
			{
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Joker),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Joker)
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_Quints_Permutation()
		{
			string handString = "DDDDDb NNNN 11111b consec anyWind";
			List<Tile> tiles = new()
			{
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Joker),
				new(Kind.FlowerWind, wind: Wind.East),
				new(Kind.FlowerWind, wind: Wind.East),
				new(Kind.FlowerWind, wind: Wind.East),
				new(Kind.FlowerWind, wind: Wind.East),
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new(Kind.Joker)
			};
			// TODO: flex winds
			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_Quints_WithJokers()
		{
			string handString = "DDDDDb NNNN 11111b consec";
			List<Tile> tiles = new()
			{
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Joker)
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_Quints_Invalid()
		{
			string handString = "DDDDDb NNNN 11111b consec";
			List<Tile> tiles = new()
			{
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Crak), // invalid
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.FlowerWind, wind: Wind.North),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Number, num: 1, suit: Suit.Dot),
				new(Kind.Joker)
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsFalse(result);
		}

		[Test]
		public void CheckHand_Dragon2022_ExactMatch()
		{
			string handString = "FF GGGG 2 0 2 2 RRRR";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_Dragon2022_Permutation()
		{
			string handString = "FF GGGG 2 0 2 2 RRRR";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Number, num: 2, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Number, num: 2, suit: Suit.Crak),
				new(Kind.Number, num: 2, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_Dragon2022_WithJokers()
		{
			string handString = "FF GGGG 2 0 2 2 RRRR";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Dragon, suit: Suit.Bam),
				new(Kind.Joker),
				new(Kind.Joker),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_Dragon2022_Invalid()
		{
			string handString = "FF GGGG 2 0 2 2 RRRR";
			List<Tile> tiles = new()
			{
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.FlowerWind, wind: Wind.Flower),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Dot),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Bam), // 0 can't be bam
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Number, num: 2, suit: Suit.Dot),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
				new(Kind.Dragon, suit: Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsFalse(result);
		}

		[Test]
		public void CheckHand_OddLike_ExactMatch()
		{
			string handString = "1111g NN E W SS 1111r odd";
			List<Tile> tiles = new()
			{
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new(Kind.Number, num: 1, suit: Suit.Bam),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.East),
				new (Kind.FlowerWind, wind: Wind.West),
				new (Kind.FlowerWind, wind: Wind.South),
				new (Kind.FlowerWind, wind: Wind.South),
				new(Kind.Number, num: 1, suit: Suit.Crak),
				new(Kind.Number, num: 1, suit: Suit.Crak),
				new(Kind.Number, num: 1, suit: Suit.Crak),
				new(Kind.Number, num: 1, suit: Suit.Crak),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_OddLike_Permutation()
		{
			string handString = "1111g NN E W SS 1111r odd";
			List<Tile> tiles = new()
			{
				new(Kind.Number, num: 5, suit: Suit.Bam),
				new(Kind.Number, num: 5, suit: Suit.Bam),
				new(Kind.Number, num: 5, suit: Suit.Bam),
				new(Kind.Number, num: 5, suit: Suit.Bam),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.East),
				new (Kind.FlowerWind, wind: Wind.West),
				new (Kind.FlowerWind, wind: Wind.South),
				new (Kind.FlowerWind, wind: Wind.South),
				new(Kind.Number, num: 5, suit: Suit.Dot),
				new(Kind.Number, num: 5, suit: Suit.Dot),
				new(Kind.Number, num: 5, suit: Suit.Dot),
				new(Kind.Number, num: 5, suit: Suit.Dot),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_OddLike_WithJokers()
		{
			string handString = "1111g NN E W SS 1111r odd";
			List<Tile> tiles = new()
			{
				new(Kind.Number, num: 5, suit: Suit.Bam),
				new(Kind.Number, num: 5, suit: Suit.Bam),
				new(Kind.Joker),
				new(Kind.Number, num: 5, suit: Suit.Bam),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.East),
				new (Kind.FlowerWind, wind: Wind.West),
				new (Kind.FlowerWind, wind: Wind.South),
				new (Kind.FlowerWind, wind: Wind.South),
				new(Kind.Number, num: 5, suit: Suit.Dot),
				new(Kind.Number, num: 5, suit: Suit.Dot),
				new(Kind.Number, num: 5, suit: Suit.Dot),
				new(Kind.Number, num: 5, suit: Suit.Dot),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsTrue(result);
		}

		[Test]
		public void CheckHand_OddLike_Invalid()
		{
			string handString = "1111g NN E W SS 1111r odd";
			List<Tile> tiles = new()
			{
				new(Kind.Number, num: 4, suit: Suit.Bam), // even is invalid
				new(Kind.Number, num: 4, suit: Suit.Bam),
				new(Kind.Number, num: 4, suit: Suit.Bam),
				new(Kind.Number, num: 4, suit: Suit.Bam),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.North),
				new (Kind.FlowerWind, wind: Wind.East),
				new (Kind.FlowerWind, wind: Wind.West),
				new (Kind.FlowerWind, wind: Wind.South),
				new (Kind.FlowerWind, wind: Wind.South),
				new(Kind.Number, num: 4, suit: Suit.Dot),
				new(Kind.Number, num: 4, suit: Suit.Dot),
				new(Kind.Number, num: 4, suit: Suit.Dot),
				new(Kind.Number, num: 4, suit: Suit.Dot),
			};

			bool result = HandChecker.CheckHand(tiles, handString);

			Assert.IsFalse(result);
		}
	}
}