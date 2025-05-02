using System;
using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public class CharlestonHandler
	{
		private int _playersReady = 0;
		private readonly int[] _numTilesPassedByPlayer = new int[4];
		private readonly int[][] _passArr = { new int[4], new int[4], new int[4] };
		private readonly int[] _passDir = { 1, 0, -1, -1, 0, 1, 0 };
		private int _passNum;
		private readonly int[] _partialPasses = { 2, 5, 6 };
		private readonly int[][] _passResult = { new int[3], new int[3], new int[3], new int[3] };
		private readonly TileTrackerServer _tileTracker;
		
		// _passArr is an array
		// of the form:
		//	{
		//		{ P1T1, P2T1, P3T1, P4T1 },
		//		{ P1T2, P2T2, P3T2, P4T2 },
		//		{ P1T3, P2T3, P3T3, P4T3 }
		//	}
		// for partial passes:
		//	{
		//		{ P1T1, P2T1, P3T1, P4T1 }, =>	{ P4T1, P1T1, P2T1, P3T1 },
		//		{ P1T2, P2T2, P3T2, P4T2 }, =>	{ P4T2, P1T2, P2T2, P3T2 },
		//		{ P1T3, P3T3,		P4T3 }	=>	{ P4T3, P1T3,		P3T3 }
		//	}

		public CharlestonHandler(TileTrackerServer tileTracker)
		{
			_tileTracker = tileTracker;
		}
		
		public void ReceiveTilesFromPlayer(int playerIx, int[] tiles)
		{
			// validation
			
			// make sure the tiles passed are in the player's rack
			foreach (int tileId in tiles)
			{
				if (tileId != -1 && !_tileTracker.PlayerPrivateRackContains(playerIx, tileId))
				{
					throw new UnityEngine.UnityException($"Tile {tileId} is not in player {playerIx}'s rack");
				}
			}
			
			// make sure array contains three (even if some elements == -1)
			if (tiles == null || tiles.Length != 3)
			{
				throw new UnityEngine.UnityException("Tiles array must contain exactly 3 elements");
			}

			// make sure playerIx is valid
			if (playerIx is < 0 or >= 4)
			{
				throw new UnityEngine.UnityException("Player index must be between 0 and 3");
			}

			// update _passArr and _numTilesPassedByPlayer
			for (int i = 0; i < 3; i++)
			{
				_passArr[i][playerIx] = tiles[i];
				if (tiles[i] != -1) _numTilesPassedByPlayer[playerIx]++;
			}
			
			// if less than 3 tiles passed and not a partial pass, throw exception
			if (!_partialPasses.Contains(_passNum) && _numTilesPassedByPlayer[playerIx] < 3)
			{
				throw new UnityEngine.UnityException("Players passed < 3 tiles on non-partial passes");
			}

			// do the pass if all players have submitted
			_playersReady++;
			if (_playersReady == 4) DoPass();
		}

		private void DoPass()
		{
			// Debug.Log($"Doing pass {_passNum}");
			// Build pass result
			List<List<int>> passList = new();
			
			foreach (int[] arr in _passArr)
			{
				List<int> pass = arr.Where(tileId => tileId != -1).ToList();
				if (pass.Count == 0) break;
				// pass right
				if (_passDir[_passNum] == 1)
				{
					int last = pass.Last();
					pass.RemoveAt(pass.Count - 1);
					pass.Insert(0, last);
				}
				// pass left
				if (_passDir[_passNum] == -1)
				{
					int first = pass.First();
					pass.RemoveAt(0);
					pass.Add(first);
				}
				// pass across - you can assume all 4 are populated
				if (_passDir[_passNum] == 0)
				{
					List<int> passHalf = pass.Take(2).ToList();
					pass = pass.Skip(2).ToList();
					pass.AddRange(passHalf);
				}
				passList.Add(pass);
			}
			
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				for (int i = 0; i < _numTilesPassedByPlayer[playerIx]; i++)
				{
					_passResult[playerIx][i] = passList[i][0]; // pick from the front since we're removing each time
					passList[i].RemoveAt(0);
				}
			}
			
			// Send tile to players
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				foreach (int tileId in _passResult[playerIx])
				{
					_tileTracker.MoveTile(tileId, _tileTracker.GetPrivateRackForPlayer(playerIx), sendToAll: false);
				}
			}
			_tileTracker.SendGameStateToAll();
			
			// Reset state for next pass
			_playersReady = 0;
			Array.Clear(_numTilesPassedByPlayer, 0, _numTilesPassedByPlayer.Length);
			
			// Check if we've completed all passes
			_passNum++;
			if (_passNum >= _passDir.Length)
			{
				OnCharlestonComplete();
			}
		}

		private void OnCharlestonComplete()
		{
			//UnityEngine.Debug.Log("OnCharlestonComplete not implemented yet");
		}
	}
}