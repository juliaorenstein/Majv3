using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Resources 
{
	public class CardParser
	{
		// TODO: handle evens and odds
		public Hand GetBaseHand(string handStr)
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

		public List<Hand> PermutateNum(Hand baseHand)
		{
			List<Hand> hands = new() { baseHand };
			int max = baseHand.Groups.Where(gr => gr.Kind == Kind.Number).Max(gr => gr.Number);
			int increment = baseHand.EvenOdd ? 2 : 1;
			for (int i = 1; i + max <= 10; i += increment)
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

		public List<Hand> PermutateSuit(Hand baseHand)
		{
			// TODO: for 1-suit hands this will created duplicates
			List<Hand> hands = new() { baseHand };

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
		public bool EvenOdd;

		public Hand()
		{
			Groups = new();
		}

		public Hand(List<Group> groups, bool permutateNum, bool permutateSuit)
		{
			Groups = groups;
			PermutateNum = permutateNum;
			PermutateSuit = permutateSuit;
		}

		public Hand(Hand hand)
		{
			Groups = new(hand.Groups);
			PermutateNum = hand.PermutateNum;
			PermutateSuit = hand.PermutateSuit;
		}

		public bool IsSameAs(Hand hand) // use instead of equals to compare hand contents
		{
			if (this == hand) return true;
			if (hand.Groups.Count != Groups.Count) return false;
			for (int i = 0; i < Groups.Count; i++)
			{
				if (!Groups[i].Equals(hand.Groups[i])) return false;
			}
			return PermutateNum == hand.PermutateNum & PermutateSuit == hand.PermutateSuit;
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
}