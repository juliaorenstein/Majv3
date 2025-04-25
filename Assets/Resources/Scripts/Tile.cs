using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resources
{
	public class Tile
	{
		public static List<Tile> AllTiles = new();
		
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

		public Tile(Kind kind, int id = -1, int num = -1, Suit suit = Suit.None, Wind wind = Wind.None)
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
	}

	public enum Kind
	{
		Number,
		Dragon,
		FlowerWind,
		Joker
	}

	public enum Suit
	{
		Bam,
		Crak,
		Dot,
		None
	}

	public enum Wind
	{
		North,
		South,
		East,
		West,
		Flower,
		None
	}
}