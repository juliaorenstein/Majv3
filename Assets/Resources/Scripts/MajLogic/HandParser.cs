using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Resources 
{
	public static class HandParser
	{
		public static List<Hand> GetAllPermutations(string handStr)
		{
			Hand baseHand = Hand.GetBaseHand(handStr);
			return GetAllPermutations(baseHand);
		}
		
		public static List<Hand> GetAllPermutations(Hand baseHand)
		{
			List<Hand> numberPermutations = PermutateNum(baseHand);
			List<Hand> suitPermutations = new();
			foreach (Hand hand in numberPermutations)
			{
				suitPermutations.AddRange(PermutateSuit(hand));
			}
			List<Hand> allPermutations = new();
			foreach (Hand hand in suitPermutations)
			{
				allPermutations.AddRange(PermutateWind(hand));
			}
			return allPermutations;
		}
		
		public static List<Hand> PermutateNum(Hand baseHand)
		{
			List<Hand> hands = new() { baseHand };
			if (!baseHand.PermutateNum) return hands;
			
			int max = baseHand.Groups.Where(gr => gr.Kind == Kind.Number).Max(gr => gr.Number);
			int initial = baseHand.EvenOdd ? 2 : 1;
			int increment = baseHand.EvenOdd ? 2 : 1;
			
			for (int i = initial; i + max < 10; i += increment)
			{
				Hand newHand = new(baseHand) { Groups = new() };
				foreach (Group baseGroup in baseHand.Groups)
				{
					if (baseGroup.Kind is Kind.Number)
					{
						newHand.Groups.Add(new() { Count = baseGroup.Count, Kind = baseGroup.Kind
							, Number = baseGroup.Number + i, Suit = baseGroup.Suit });
					}
					else newHand.Groups.Add(baseGroup);
				}
				hands.Add(newHand);
			}
			return hands;
		}

		public static List<Hand> PermutateSuit(Hand baseHand)
		{
			// TODO: for 1-suit hands this will created duplicates
			List<Hand> hands = new() { baseHand };
			if (!baseHand.PermutateSuit) return hands;

			List<Dictionary<Suit, Suit>> suitPermutator = new()
			{
				new() {
					{ Suit.Bam, Suit.Crak },
					{ Suit.Crak, Suit.Dot },
					{ Suit.Dot, Suit.Bam }
				},
				new() {
					{ Suit.Bam, Suit.Dot },
					{ Suit.Crak, Suit.Bam },
					{ Suit.Dot, Suit.Crak }
				},
				new() {
					{ Suit.Bam, Suit.Bam },
					{ Suit.Crak, Suit.Dot },
					{ Suit.Dot, Suit.Crak }
				},
				new() {
					{ Suit.Bam, Suit.Dot },
					{ Suit.Crak, Suit.Crak },
					{ Suit.Dot, Suit.Bam }
				},
				new() {
					{ Suit.Bam, Suit.Crak },
					{ Suit.Crak, Suit.Bam },
					{ Suit.Dot, Suit.Dot }
				}
			};
			
			for (int i = 0; i < 5; i++)
			{
				Hand newHand = new(baseHand) { Groups = new() };
				foreach (Group baseGroup in baseHand.Groups)
				{
					
					if (baseGroup.Kind is Kind.Number or Kind.Dragon && !baseGroup.NonFlexDragon)
					{
						newHand.Groups.Add(new() { Count = baseGroup.Count, Kind = baseGroup.Kind
							, Number = baseGroup.Number, Suit = suitPermutator[i][baseGroup.Suit] });
					}
					else newHand.Groups.Add(baseGroup);
				}
				hands.Add(newHand);
			}
			return hands;
		}
		
		public static List<Hand> PermutateWind(Hand baseHand)
		{
			// assume it starts on North and that this will only happen when 1 wind is in the hand
			List<Hand> hands = new() { baseHand };
			if (!baseHand.PermutateWind) return hands;

			List<Wind> windPermutator = new() { Wind.East, Wind.West, Wind.South };

			foreach (Wind wind in windPermutator)
			{
				Hand newHand = new(baseHand) { Groups = new() };
				foreach (Group baseGroup in baseHand.Groups)
				{
					if (baseGroup.Kind is Kind.FlowerWind && baseGroup.Wind is not Wind.Flower)
					{
						newHand.Groups.Add(new() { Count = baseGroup.Count, Kind = baseGroup.Kind, Wind = wind });
					}
					else newHand.Groups.Add(baseGroup);
				}
				hands.Add(newHand);
			}
			return hands;
		}
	}
	
	public struct Group
	{
		public int Count;
		public Kind Kind;
		public int Number;
		public Suit Suit;
		public Wind Wind;
		public bool NonFlexDragon;

		public override string ToString()
		{
			switch (Kind)
			{
				case Kind.FlowerWind:
					return $"{Wind} ({Count})";
				case Kind.Dragon:
				{
					string color = Suit switch
					{
						Suit.Bam => "Green",
						Suit.Crak => "Red",
						Suit.Dot => "Soap",
						_ => "Invalid dragon"
					};
					return $"{color} ({Count})";
				}
				case Kind.Number:
					return $"{Number} {Suit} ({Count})";
				case Kind.Joker:
				default:
					throw new UnityException(
						$"Invalid Group: Kind-{Kind}; Number-{Number}; Suit-{Suit}; Wind-{Wind}; Count-{Count}");
			}
		}

		public bool Equals(Group other)
		{
			return Count == other.Count
			       && Kind == other.Kind 
			       && Number == other.Number 
			       && Suit == other.Suit 
			       && Wind == other.Wind 
			       && NonFlexDragon == other.NonFlexDragon;
		}

		public override bool Equals(object obj)
		{
			return obj is Group other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Count, (int)Kind, Number, (int)Suit, (int)Wind, NonFlexDragon);
		}

		public static bool operator ==(Group left, Group right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Group left, Group right)
		{
			return !left.Equals(right);
		}
	}

	public class Hand
	{
		public List<Group> Groups;
		public bool PermutateNum;
		public bool PermutateSuit;
		public bool PermutateWind;
		public bool EvenOdd;

		public Hand()
		{
			Groups = new();
		}

		public Hand(List<Group> groups, bool permutateNum, bool permutateSuit
			, bool permutateWind, bool evenOdd = false)
		{
			Groups = groups;
			PermutateNum = permutateNum;
			PermutateSuit = permutateSuit;
			PermutateWind = permutateWind;
			EvenOdd = evenOdd;
		}

		public Hand(Hand hand)
		{
			Groups = new(hand.Groups);
			PermutateNum = hand.PermutateNum;
			PermutateSuit = hand.PermutateSuit;
			PermutateWind = hand.PermutateWind;
			EvenOdd = hand.EvenOdd;
		}
		
		public static Hand GetBaseHand(string handStr)
		{
			string[] groupStrings = handStr.Split(" ");
			Hand baseHand = new();

			foreach (string str in groupStrings)
			{
				switch (str)
				{
					case "consec":
						baseHand.PermutateNum = true;
						continue;
					case "even" or "odd":
						baseHand.PermutateNum = true;
						baseHand.EvenOdd = true;
						continue;
					case "anyWind":
						baseHand.PermutateWind = true;
						continue;
				}
				
				Group group = new();
				char firstChar = str.First();
				char lastChar = str.Last();
				
				group.Count = char.IsLower(lastChar) ? str.Length - 1 : str.Length;
				
				group.Kind = firstChar switch
				{
					'F' => Kind.FlowerWind,
					'N' => Kind.FlowerWind,
					'E' => Kind.FlowerWind,
					'W' => Kind.FlowerWind,
					'S' => Kind.FlowerWind,
					'D' => Kind.Dragon,
					'0' => Kind.Dragon,
					'G' => Kind.Dragon,
					'R' => Kind.Dragon,
					_ => Kind.Number
				};
				
				// FLOWER WIND
				if (group.Kind == Kind.FlowerWind)
				{
					group.Wind = firstChar switch
					{
						'F' => Wind.Flower,
						'N' => Wind.North,
						'E' => Wind.East,
						'W' => Wind.West,
						'S' => Wind.South,
						_ => throw new UnityException("Invalid wind")
					};
				}
				
				// DRAGON
				else if (group.Kind == Kind.Dragon)
				{
					baseHand.PermutateSuit = true;
					group.Suit = firstChar switch
					{
						'G' => Suit.Bam,
						'R' => Suit.Crak,
						'0' => Suit.Dot,
						_ => Suit.None
					};

					// if suit is assigned, this is a non-flexible dragon
					group.NonFlexDragon = group.Suit != Suit.None;

					group.Suit = group.Suit == Suit.None ? lastChar switch
					{
						'g' => Suit.Bam,
						'r' => Suit.Crak,
						_ => Suit.Dot
					} : group.Suit;
				}

				// NUMBER
				else
				{
					baseHand.PermutateSuit = true;
					int.TryParse(firstChar.ToString(), out group.Number);
					if (group.Number == default) throw new UnityException("Invalid number");
					group.Suit = lastChar switch
					{
						'g' => Suit.Bam,
						'r' => Suit.Crak,
						_ => Suit.Dot
					};
				}
				baseHand.Groups.Add(group);
			}
			return baseHand;
		}

		public bool IsSameAs(Hand hand) // use instead of equals to compare hand contents
		{
			if (this == hand) return true;
			if (hand.Groups.Count != Groups.Count) return false;
			for (int i = 0; i < Groups.Count; i++)
			{
				if (!Groups[i].Equals(hand.Groups[i])) return false;
			}
			return PermutateNum == hand.PermutateNum
				&& PermutateSuit == hand.PermutateSuit
				&& EvenOdd == hand.EvenOdd;
		}
		
		public override string ToString()
		{
			StringBuilder str = new();
			foreach (Group group in Groups)
			{
				str.Append($" > {group}\n");
			}
			str.Append("PermutateNum: " + PermutateNum);
			str.Append("\nPermutateSuit: " + PermutateSuit);
			str.Append("\nEvenOdd: " + EvenOdd);
			return str.ToString();
		}
	}
	
	public class HandComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			if (x is Hand hand1 && y is Hand hand2)
			{
				return hand1.IsSameAs(hand2) ? 0 : -1;
			}
			throw new UnityException("Objects being compared are not of type 'Hand'.");
		}
	}
}