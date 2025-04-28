using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resources
{
	public static class HandChecker
	{
		public static bool CheckHand(List<Tile> tiles, string handStr)
		{
			if (tiles.Count != 14) return false;
							
			List<Hand> permutations = HandParser.GetAllPermutations(handStr);
			Debug.Assert(permutations.Count > 0);

			return permutations.Any(hand => CheckPermutation(new(tiles), hand));
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