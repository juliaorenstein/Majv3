using System.Collections.Generic;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Resources.MajLogicTests
{
	public class HandParserTests
	{
		[Test]
		public void Hand_IsSameAs_True()
		{
			List<Group> groups = new()
			{
				new Group { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.North },
				new Group { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam }
			};
			Hand hand1 = new Hand(groups, true, false, false, true);
			Hand hand2 = new Hand(new(groups), true, false, false, true);

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
			Hand hand1 = new Hand(groups1, true, false, true, false);
			Hand hand2 = new Hand(groups2, true, false, true, false);

			Assert.False(hand1.IsSameAs(hand2));
		}

		[Test]
		public void GetBaseHand_BasicConsec()
		{
			string testHand = "FFF 1111g FFF 2222r flexNum";
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

			Hand actualBaseHand = Hand.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void GetBaseHand_Quints()
		{
			string testHand = "DDDDDb NNNN 11111b flexNum";
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

			Hand actualBaseHand = Hand.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void GetBaseHand_Dragon2022()
		{
			string testHand = "FF GGGG 2 0 2 2 RRRR";
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

			Hand actualBaseHand = Hand.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void GetBaseHand_OddLike()
		{
			string testHand = "1111g NN E W SS 1111r odd";
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

			Hand actualBaseHand = Hand.GetBaseHand(testHand);

			Assert.True(expectedBaseHand.IsSameAs(actualBaseHand)
				, $"expected:\n{expectedBaseHand}\n\nactual\n{actualBaseHand}");
		}

		[Test]
		public void PermutateNum_BasicConsec()
		{
			Hand testBaseHand = Hand.GetBaseHand("FFF 1111g FFF 2222r flexNum");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("FFF 2222g FFF 3333r flexNum"),
				Hand.GetBaseHand("FFF 3333g FFF 4444r flexNum"),
				Hand.GetBaseHand("FFF 4444g FFF 5555r flexNum"),
				Hand.GetBaseHand("FFF 5555g FFF 6666r flexNum"),
				Hand.GetBaseHand("FFF 6666g FFF 7777r flexNum"),
				Hand.GetBaseHand("FFF 7777g FFF 8888r flexNum"),
				Hand.GetBaseHand("FFF 8888g FFF 9999r flexNum")
			};

			List<Hand> actualResults = HandParser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateNum_Quints()
		{
			Hand testBaseHand = Hand.GetBaseHand("DDDDDb NNNN 11111b flexNum");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("DDDDDb NNNN 22222b flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 33333b flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 44444b flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 55555b flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 66666b flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 77777b flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 88888b flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 99999b flexNum")
			};

			List<Hand> actualResults = HandParser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateNum_Dragon2022()
		{
			Hand testBaseHand = Hand.GetBaseHand("FF GGGG 2 0 2 2 RRRR");

			List<Hand> expectedResults = new() { testBaseHand };

			List<Hand> actualResults = HandParser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}

		[Test]
		public void PermutateNum_OddLike()
		{
			Hand testBaseHand = Hand.GetBaseHand("1111g NN E W SS 1111r odd");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("3333g NN E W SS 3333r odd"),
				Hand.GetBaseHand("5555g NN E W SS 5555r odd"),
				Hand.GetBaseHand("7777g NN E W SS 7777r odd"),
				Hand.GetBaseHand("9999g NN E W SS 9999r odd")
			};

			List<Hand> actualResults = HandParser.PermutateNum(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		} 
		
		[Test]
		public void PermutateSuit_BasicConsec()
		{
			Hand testBaseHand = Hand.GetBaseHand("FFF 1111g FFF 2222r flexNum");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("FFF 1111r FFF 2222b flexNum"),
				Hand.GetBaseHand("FFF 1111b FFF 2222g flexNum"),
				Hand.GetBaseHand("FFF 1111g FFF 2222b flexNum"),
				Hand.GetBaseHand("FFF 1111b FFF 2222r flexNum"),
				Hand.GetBaseHand("FFF 1111r FFF 2222g flexNum")
			};

			List<Hand> actualResults = HandParser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateSuit_Quints()
		{
			Hand testBaseHand = Hand.GetBaseHand("DDDDDb NNNN 11111b flexNum");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("DDDDDg NNNN 11111g flexNum"),
				Hand.GetBaseHand("DDDDDr NNNN 11111r flexNum"),
				Hand.GetBaseHand("DDDDDr NNNN 11111r flexNum"),
				Hand.GetBaseHand("DDDDDg NNNN 11111g flexNum"),
				Hand.GetBaseHand("DDDDDb NNNN 11111b flexNum"),
			};

			List<Hand> actualResults = HandParser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateSuit_Dragon2022()
		{
			Hand testBaseHand = Hand.GetBaseHand("FF GGGG 2 0 2 2 RRRR");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("FF GGGG 2g 0 2g 2g RRRR"),
				Hand.GetBaseHand("FF GGGG 2r 0 2r 2r RRRR"),
				Hand.GetBaseHand("FF GGGG 2r 0 2r 2r RRRR"),
				Hand.GetBaseHand("FF GGGG 2g 0 2g 2g RRRR"),
				Hand.GetBaseHand("FF GGGG 2b 0 2b 2b RRRR")
			};

			List<Hand> actualResults = HandParser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}

		[Test]
		public void PermutateSuit_OddLike()
		{
			Hand testBaseHand = Hand.GetBaseHand("1111g NN E W SS 1111r odd");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("1111r NN E W SS 1111b odd"),
				Hand.GetBaseHand("1111b NN E W SS 1111g odd"),
				Hand.GetBaseHand("1111g NN E W SS 1111b odd"),
				Hand.GetBaseHand("1111b NN E W SS 1111r odd"),
				Hand.GetBaseHand("1111r NN E W SS 1111g odd")
			};

			List<Hand> actualResults = HandParser.PermutateSuit(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}

		[Test]
		public void PermutateWind_Quints_4Results()
		{
			Hand testBaseHand = Hand.GetBaseHand("DDDDDb NNNN 11111b flexNum flexWind");

			List<Hand> expectedResults = new()
			{
				testBaseHand,
				Hand.GetBaseHand("DDDDDb EEEE 11111b flexNum flexWind"),
				Hand.GetBaseHand("DDDDDb WWWW 11111b flexNum flexWind"),
				Hand.GetBaseHand("DDDDDb SSSS 11111b flexNum flexWind"),
			};
			
			List<Hand> actualResults = HandParser.PermutateWind(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}
		
		[Test]
		public void PermutateWind_BasicConsec_1Result()
		{
			Hand testBaseHand = Hand.GetBaseHand("FFF 1111g FFF 2222r flexNum");

			List<Hand> expectedResults = new() { testBaseHand, };

			List<Hand> actualResults = HandParser.PermutateWind(testBaseHand);

			CollectionAssert.AreEqual(expectedResults, actualResults, new HandComparer());
		}

		[Test]
		public void PermutateAll_BasicConsec()
		{
			Hand testBaseHand = Hand.GetBaseHand("FFF 1111g FFF 2222r flexNum");

			List<Hand> results = HandParser.GetAllPermutations(testBaseHand);
			
			Assert.AreEqual(48, results.Count);
		}
	}
}