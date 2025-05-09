using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resources
{
	public class Tile : IComparable<Tile>, IComparable
	{
		public static readonly List<Tile> AllTiles = new();
		
		// properties
		public int Id { get; private set; }
		public Kind Kind { get; private set; }
		public int Number { get; private set; }
		public Suit Suit { get; private set; }
		public Wind Wind { get; private set; }
		
		// mono reference
		public Transform TileTransform { get; set; }
		public bool IsMono => TileTransform != null;

		// static items
		public static List<Suit> Suits { get; private set; } = new()
		{
			Suit.Bam,
			Suit.Crak,
			Suit.Dot
		};
		public static List<Wind> Winds { get; private set; } = new()
		{
			Wind.North,
			Wind.South,
			Wind.East,
			Wind.West,
			Wind.Flower
		};

		public Tile(Kind kind, int id = -1, int num = 0, Suit suit = Suit.None, Wind wind = Wind.None)
		{
			Kind = kind;
			Number = num;
			Suit = suit;
			Wind = wind;
			Id = id;
			AllTiles.Add(this);
		}
		
		public bool IsJoker() => Kind == Kind.Joker;
		public static bool IsJoker(int tileId) => AllTiles[tileId].IsJoker();

		public static bool AreSame(int tileId1, int tileId2, bool jokerValid = true)
		{
			Tile tile1 = AllTiles[tileId1];
			Tile tile2 = AllTiles[tileId2];
			if (jokerValid && (tile1.IsJoker() || tile2.IsJoker())) return true;
			return tile1.Kind == tile2.Kind 
			       && tile1.Number == tile2.Number
			       && tile1.Suit == tile2.Suit
			       && tile1.Wind == tile2.Wind;
		}

		public override string ToString() => Kind switch
		{
			Kind.Dragon => DragonToString,
			Kind.Number => $"{Number} {Suit}",
			Kind.FlowerWind => Wind.ToString(),
			Kind.Joker => "Joker",
			_ => throw new Exception($"Unknown kind")
		};

		private string DragonToString => Suit switch
		{
			Suit.Bam => "Green",
			Suit.Crak => "Red",
			Suit.Dot => "Soap",
			_ => throw new Exception($"Unknown suit")
		};

		public int CompareTo(Tile other)
		{
			if (ReferenceEquals(this, other)) return 0;
			if (other is null) return 1;
			// compare by id in real gameplay
			var idComparison = Id.CompareTo(other.Id);
			// in unit tests this will do the same (hopefully!)
			if (idComparison != 0) return idComparison;
			var kindComparison = Kind.CompareTo(other.Kind);
			if (kindComparison != 0) return kindComparison;
			var suitComparison = Suit.CompareTo(other.Suit);
			if (suitComparison != 0) return suitComparison;
			var numberComparison = Number.CompareTo(other.Number);
			if (numberComparison != 0) return numberComparison;
			return Wind.CompareTo(other.Wind);
		}

		public int CompareTo(object obj)
		{
			if (obj is null) return 1;
			if (ReferenceEquals(this, obj)) return 0;
			return obj is Tile other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Tile)}");
		}
	}

	public enum Kind
	{
		Dragon,
		Number,
		FlowerWind,
		Joker
	}

	public enum Suit
	{
		None,
		Bam,
		Crak,
		Dot
	}

	public enum Wind
	{
		None,
		North,
		South,
		East,
		West,
		Flower
	}
}