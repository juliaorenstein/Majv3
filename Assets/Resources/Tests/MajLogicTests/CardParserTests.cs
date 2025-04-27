using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using NUnit.Framework;

namespace Resources.MajLogicTests
{
	public class CardParserTests
	{
		[Test]
		public void Hand_IsSameAs_True()
		{
			List<Group> groups = new()
			{
				new Group { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.North },
				new Group { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam }
			};
			Hand hand1 = new Hand(groups, true, false);
			Hand hand2 = new Hand(new(groups), true, false);

			Assert.True(hand1.IsSameAs(hand2));
		}

		[Test]
		public void Hand_IsSameAs_False()
		{
			List<Group> groups1 = new()
			{
				new Group { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.North },
				new Group { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam }
			};
			List<Group> groups2 = new()
			{
				new Group { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam },
				new Group { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.North }
			};
			Hand hand1 = new Hand(groups1, true, false);
			Hand hand2 = new Hand(groups2, true, false);

			Assert.False(hand1.IsSameAs(hand2));
		}

		[Test]
		public void GetBaseHand_BasicConsec()
		{
			string testHand = "FFF 1111g FFF 2222r consec";
			CardParser parser = new();
			Hand expectedBaseHand = new()
			{
				Groups = new()
				{
					new() { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.Flower },
					new() { Count = 4, Kind = Kind.Number, Number = 1, Suit = Suit.Bam },
					new() { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.Flower },
					new() { Count = 4, Kind = Kind.Number, Number = 2, Suit = Suit.Crak }
				},
				PermutateNum = true,
				PermutateSuit = true
			};

			Hand actualBaseHand = parser.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void GetBaseHand_Quints()
		{
			string testHand = "DDDDDb NNNN 11111b consec";
			CardParser parser = new();
			Hand expectedBaseHand = new()
			{
				Groups = new()
				{
					new() { Count = 5, Kind = Kind.Dragon, Suit = Suit.Dot },
					new() { Count = 4, Kind = Kind.FlowerWind, Wind = Wind.North },
					new() { Count = 5, Kind = Kind.Number, Number = 1, Suit = Suit.Dot }
				},
				PermutateNum = true,
				PermutateSuit = true
			};

			Hand actualBaseHand = parser.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void GetBaseHand_Dragon2022()
		{
			string testHand = "FF GGGG 2 0 2 2 RRRR";
			CardParser parser = new();
			Hand expectedBaseHand = new()
			{
				Groups = new()
				{
					new() { Count = 2, Kind = Kind.FlowerWind, Wind = Wind.Flower },
					new() { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam, NonFlexDragon = true },
					new() { Count = 1, Kind = Kind.Number, Number = 2, Suit = Suit.Dot },
					new() { Count = 1, Kind = Kind.Dragon, Suit = Suit.Dot, NonFlexDragon = true },
					new() { Count = 1, Kind = Kind.Number, Number = 2, Suit = Suit.Dot },
					new() { Count = 1, Kind = Kind.Number, Number = 2, Suit = Suit.Dot },
					new() { Count = 4, Kind = Kind.Dragon, Suit = Suit.Crak, NonFlexDragon = true }
				},
				PermutateSuit = true
			};

			Hand actualBaseHand = parser.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void GetBaseHand_OddLike()
		{
			string testHand = "1111g NN E W SS 1111r odd";
			CardParser parser = new();
			Hand expectedBaseHand = new()
			{
				Groups = new()
				{
					new() { Count = 4, Kind = Kind.Number, Number = 1, Suit = Suit.Bam },
					new() { Count = 2, Kind = Kind.FlowerWind, Wind = Wind.North },
					new() { Count = 1, Kind = Kind.FlowerWind, Wind = Wind.East },
					new() { Count = 1, Kind = Kind.FlowerWind, Wind = Wind.West },
					new() { Count = 2, Kind = Kind.FlowerWind, Wind = Wind.South },
					new() { Count = 4, Kind = Kind.Number, Number = 1, Suit = Suit.Crak },
				},
				PermutateNum = true,
				PermutateSuit = true,
				EvenOdd = true
			};

			Hand actualBaseHand = parser.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void PermutateNum_BasicConsec()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("FFF 1111g FFF 2222r consec");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				parser.GetBaseHand("FFF 2222g FFF 3333r consec"),
				parser.GetBaseHand("FFF 3333g FFF 4444r consec"),
				parser.GetBaseHand("FFF 4444g FFF 5555r consec"),
				parser.GetBaseHand("FFF 5555g FFF 6666r consec"),
				parser.GetBaseHand("FFF 6666g FFF 7777r consec"),
				parser.GetBaseHand("FFF 7777g FFF 8888r consec"),
				parser.GetBaseHand("FFF 8888g FFF 9999r consec")
			};

			List<Hand> actualResults = parser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateNum_Quints()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("DDDDDb NNNN 11111b consec");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				parser.GetBaseHand("DDDDDb NNNN 22222b consec"),
				parser.GetBaseHand("DDDDDb NNNN 33333b consec"),
				parser.GetBaseHand("DDDDDb NNNN 44444b consec"),
				parser.GetBaseHand("DDDDDb NNNN 55555b consec"),
				parser.GetBaseHand("DDDDDb NNNN 66666b consec"),
				parser.GetBaseHand("DDDDDb NNNN 77777b consec"),
				parser.GetBaseHand("DDDDDb NNNN 88888b consec"),
				parser.GetBaseHand("DDDDDb NNNN 99999b consec")
			};

			List<Hand> actualResults = parser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateNum_Dragon2022()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("FF GGGG 2 0 2 2 RRRR");

			List<Hand> expectedResults = new() { testBaseHand };

			List<Hand> actualResults = parser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}

		[Test]
		public void PermutateNum_OddLike()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("1111g NN E W SS 1111r odd");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				parser.GetBaseHand("3333g NN E W SS 3333r odd"),
				parser.GetBaseHand("5555g NN E W SS 5555r odd"),
				parser.GetBaseHand("7777g NN E W SS 7777r odd"),
				parser.GetBaseHand("9999g NN E W SS 9999r odd")
			};

			List<Hand> actualResults = parser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		} 
		
		[Test]
		public void PermutateSuit_BasicConsec()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("FFF 1111g FFF 2222r consec");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				parser.GetBaseHand("FFF 1111r FFF 2222b consec"),
				parser.GetBaseHand("FFF 1111b FFF 2222g consec"),
				parser.GetBaseHand("FFF 1111g FFF 2222b consec"),
				parser.GetBaseHand("FFF 1111b FFF 2222r consec"),
				parser.GetBaseHand("FFF 1111r FFF 2222g consec")
			};

			List<Hand> actualResults = parser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		[Test]
		public void PermutateSuit_Quints()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("DDDDDb NNNN 11111b consec");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				parser.GetBaseHand("DDDDDg NNNN 11111g consec"),
				parser.GetBaseHand("DDDDDr NNNN 11111r consec"),
				parser.GetBaseHand("DDDDDr NNNN 11111r consec"),
				parser.GetBaseHand("DDDDDg NNNN 11111g consec"),
				parser.GetBaseHand("DDDDDb NNNN 11111b consec"),
			};

			List<Hand> actualResults = parser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateSuit_Dragon2022()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("FF GGGG 2 0 2 2 RRRR");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				parser.GetBaseHand("FF GGGG 2g 0 2g 2g RRRR"),
				parser.GetBaseHand("FF GGGG 2r 0 2r 2r RRRR"),
				parser.GetBaseHand("FF GGGG 2r 0 2r 2r RRRR"),
				parser.GetBaseHand("FF GGGG 2g 0 2g 2g RRRR"),
				parser.GetBaseHand("FF GGGG 2b 0 2b 2b RRRR")
			};

			List<Hand> actualResults = parser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}

		[Test]
		public void PermutateSuit_OddLike()
		{
			CardParser parser = new();
			Hand testBaseHand = parser.GetBaseHand("1111g NN E W SS 1111r odd");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				parser.GetBaseHand("1111r NN E W SS 1111b odd"),
				parser.GetBaseHand("1111b NN E W SS 1111g odd"),
				parser.GetBaseHand("1111g NN E W SS 1111b odd"),
				parser.GetBaseHand("1111b NN E W SS 1111r odd"),
				parser.GetBaseHand("1111r NN E W SS 1111g odd")
			};

			List<Hand> actualResults = parser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		} 

	}
}