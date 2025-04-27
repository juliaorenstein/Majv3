using System.Collections.Generic;
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
				new Group { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.North},
				new Group { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam}
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
				new Group { Count = 3, Kind = Kind.FlowerWind, Wind = Wind.North},
				new Group { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam}
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
			string testHand = "FFFF 1111g FFFF 2222r consec";
			CardParser parser = new();
			Hand expectedBaseHand = new()
			{
				Groups = new()
				{
					new() { Count = 4, Kind = Kind.FlowerWind, Wind = Wind.Flower },
					new() { Count = 4, Kind = Kind.Number, Number = 1, Suit = Suit.Bam },
					new() { Count = 4, Kind = Kind.FlowerWind, Wind = Wind.Flower },
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
					new() { Count = 4, Kind = Kind.Dragon, Suit = Suit.Bam, NonFlexDragon = true},
					new() { Count = 1, Kind = Kind.Number, Number = 2, Suit = Suit.Dot },
					new() { Count = 1, Kind = Kind.Dragon, Suit = Suit.Dot, NonFlexDragon = true},
					new() { Count = 1, Kind = Kind.Number, Number = 2, Suit = Suit.Dot },
					new() { Count = 1, Kind = Kind.Number, Number = 2, Suit = Suit.Dot },
					new() { Count = 4, Kind = Kind.Dragon, Suit = Suit.Crak, NonFlexDragon = true}
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
	}
}