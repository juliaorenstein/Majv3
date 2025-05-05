using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resources
{
	public class CharlestonHandlerServer
	{
		private readonly ICharlestonHandlerNetwork _charlestonHandlerNetwork;
		
		private readonly int[] _numTilesPassedByPlayer = new int[4];
		private readonly int[][] _passArr =
		{
			new[] { -1, -1, -1, -1},
			new[] { -1, -1, -1, -1},
			new[] { -1, -1, -1, -1}
		};
		
		private readonly int[][] _passResult = { new int[3], new int[3], new int[3], new int[3] };
		private readonly TileTrackerServer _tileTracker;
		private readonly IFusionManagerGlobal _fusionManager;
		private bool _computerPassesDone;
		
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

		public CharlestonHandlerServer(
			TileTrackerServer tileTracker, ICharlestonHandlerNetwork charlestonHandlerNetwork, IFusionManagerGlobal fusionManager)
		{
			_tileTracker = tileTracker;
			_charlestonHandlerNetwork = charlestonHandlerNetwork;
			_fusionManager = fusionManager;
		}

		public void TileToCharlestonBox(int playerIx, int tileId, int spotIx)
		{
			Debug.Log("TileToCharlestonBox");
			_charlestonHandlerNetwork.PlayersReady.Set(playerIx, false);
			
			// start computer turn if not already done
			if (!_computerPassesDone)
			{
				_computerPassesDone = true;
				for (int compPlayerIx = _fusionManager.PlayerCount; compPlayerIx < 4; compPlayerIx++)
				{
					ComputerPass(compPlayerIx);
				}
			}
			
			// validate that tileId is in playerIx's rack
			if (!_tileTracker.PlayerPrivateRackContains(playerIx, tileId))
			{
				throw new UnityException($"Tile {tileId} is not in player {playerIx}'s rack");
			}
			
			// update server-side data
			if (_passArr[spotIx][playerIx] == -1) _numTilesPassedByPlayer[playerIx]++;
			_passArr[spotIx][playerIx] = tileId;
			_charlestonHandlerNetwork.SetOccupiedSpots(playerIx, spotIx, true);
			_charlestonHandlerNetwork.CharlestonVersion++;
		}
		
		// TODO: make a CharlestonBoxToRack to clear OccupiedSpots when tile is removed
		
		public void PlayerReady(int playerIx)
		{
			// if less than 3 tiles passed and not a partial pass, throw exception
			if (!_charlestonHandlerNetwork.PartialPasses.Contains(_charlestonHandlerNetwork.PassNum) && _numTilesPassedByPlayer[playerIx] < 3)
			{
				throw new UnityEngine.UnityException("Players passed < 3 tiles on non-partial passes");
			}

			// do the pass if all players have submitted
			_charlestonHandlerNetwork.PlayersReady.Set(playerIx, true);
			if (_charlestonHandlerNetwork.PlayersReady.All(b => b)) DoPass();
			_charlestonHandlerNetwork.CharlestonVersion++;
		}

		private void DoPass()
		{
			// Build pass result
			List<List<int>> passList = new();
			
			foreach (int[] arr in _passArr)
			{
				List<int> pass = arr.Where(tileId => tileId != -1).ToList();
				if (pass.Count == 0) break;
				// pass right
				if (_charlestonHandlerNetwork.PassDir[_charlestonHandlerNetwork.PassNum] == 1)
				{
					int last = pass.Last();
					pass.RemoveAt(pass.Count - 1);
					pass.Insert(0, last);
				}
				// pass left
				if (_charlestonHandlerNetwork.PassDir[_charlestonHandlerNetwork.PassNum] == -1)
				{
					int first = pass.First();
					pass.RemoveAt(0);
					pass.Add(first);
				}
				// pass across - you can assume all 4 are populated
				if (_charlestonHandlerNetwork.PassDir[_charlestonHandlerNetwork.PassNum] == 0)
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
			
			_tileTracker.SendGameStateToAll(false);
			
			ResetState();
			
			// Check if we've completed all passes
			if (_charlestonHandlerNetwork.PassNum >= _charlestonHandlerNetwork.PassDir.Length)
			{
				OnCharlestonComplete();
			}
			return;
			
			void ResetState()
			{
				// clear tiles passed by players
				Array.Clear(_numTilesPassedByPlayer, 0, _numTilesPassedByPlayer.Length);
				// clear occupied spots
				for (int playerIx = 0; playerIx < 4; playerIx++)
				{
					for (int spotIx = 0; spotIx < 3; spotIx++)
					{
						_charlestonHandlerNetwork.SetOccupiedSpots(playerIx, spotIx, false);
					}
				}
				// clear players ready and passArr
				_charlestonHandlerNetwork.PlayersReady.Clear();
				foreach (var arr in _passArr) Array.Fill(arr, -1);
				_computerPassesDone = false;			// reset AI players
				_charlestonHandlerNetwork.PassNum++;	// increment pass num (this is how clients know to pass)
			}
		}

		private void OnCharlestonComplete()
		{
			//UnityEngine.Debug.Log("OnCharlestonComplete not implemented yet");
		}

		void ComputerPass(int playerIx)
		{
			List<int> tileIds = _tileTracker.GetPrivateRackContentsForPlayer(playerIx).ToList();
			for (int i = 0; i < 3; i++)
			{
				while (Tile.IsJoker(tileIds[i])) tileIds.RemoveAt(i); // don't pass jokers
				TileToCharlestonBox(playerIx, tileIds[i], i);
			}
			PlayerReady(playerIx);
		}
	}
}