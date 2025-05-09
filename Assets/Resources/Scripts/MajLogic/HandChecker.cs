using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resources
{
	public static class HandChecker
	{
		public static bool AnyMahJongg(List<int> tileIds, Card card)
		{
			List<Tile> tiles = tileIds.Select(tileId => Tile.AllTiles[tileId]).ToList();
			return card.BaseHands.Any(hand => CheckHand(tiles, hand));
		}

		public static bool CheckHand(List<Tile> tiles, string handStr) => CheckHand(tiles, Hand.GetBaseHand(handStr));

		private static bool CheckHand(List<Tile> tiles, Hand hand)
		{
			if (tiles.Count != 14) return false;
							
			List<Hand> permutations = HandParser.GetAllPermutations(hand);
			Debug.Assert(permutations.Count > 0);

			// handidate, like hand candidate, get it? lol
			return permutations.Any(handidate => CheckPermutation(new(tiles), handidate));
		}

		private static bool CheckPermutation(List<Tile> tilesCopy, Hand permutation)
		{
			foreach (Group group in permutation.Groups)
			{
				List<Tile> candidates = tilesCopy
					.Where(tile => TileGoesInGroup(tile, group, group.Count > 2)).ToList();

				if (candidates.Count < group.Count)
				{
					Console.WriteLine($"Not enough tiles for group {group}");
					return false;
				}
				
				// Remove tiles that form the group
				candidates.Sort();
				foreach (Tile tileToRemove in candidates.Take(group.Count))
				{
					tilesCopy.Remove(tileToRemove);
				}
			}
			return true;
		}

		private static bool TileGoesInGroup(Tile tile, Group group, bool jokersValid)
		{
			if (tile.IsJoker()) return jokersValid;

			return tile.Kind == group.Kind
			       && tile.Number == group.Number
			       && tile.Suit == group.Suit
			       && tile.Wind == group.Wind;
		}
	}
}