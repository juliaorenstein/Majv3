using System;
using Fusion;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
	public class FusionManagerGlobal : NetworkBehaviour, IFusionManagerGlobal
	{
		[Networked, Capacity(4)] public NetworkArray<PlayerRef> Players { get; }
		[Networked] public int TurnPlayerIx { get; set; }
		[Networked] public int ExposingPlayerIx { get; set; }
		[Networked] public int numTilesExposedThisTurn { get; set; }
		[Networked] public int DiscardTileId { get; set; }
		[Networked] public TurnStage CurrentTurnStage { get; set; }
		[Networked] public int MahJonggWinner { get; set; } = -1;
		
		// racks used at end of game when everybody exposes
		[Networked, Capacity(4)] public NetworkArray<int> Rack0 { get; }
		[Networked, Capacity(4)] public NetworkArray<int> Rack1 { get; }
		[Networked, Capacity(4)] public NetworkArray<int> Rack2 { get; }
		[Networked, Capacity(4)] public NetworkArray<int> Rack3 { get; }
		private NetworkArray<int>[] _racks;
		public List<int>[] Racks => _racks.Select(rack => rack.ToList()).ToArray();
		
		
		private int[] PlayerIds => Players.Select(player => player.PlayerId).ToArray();
		public int LocalPlayerIx => PlayerIx(Runner.LocalPlayer);
		public bool IsMyTurn => PlayerIx(Runner.LocalPlayer) == TurnPlayerIx;
		public bool IsMyExpose => PlayerIx(Runner.LocalPlayer) == ExposingPlayerIx;
		
		public int PlayerIx(int playerId) => Array.IndexOf(PlayerIds, playerId);
		public int PlayerIx(PlayerRef playerRef) => Array.IndexOf(PlayerIds, playerRef.PlayerId);
		public int HumanPlayerCount => Players.ToArray().Count(player => player != PlayerRef.None);
		
		public INetworkedGameState[] NetworkedGameStates { get; set; }
		public TurnManagerServer TurnManagerServer { get; set; }

		private int _readyToStart;


		public override void Spawned()
		{
			// initialize NetworkedGameState
			NetworkedGameStates = transform.GetComponentsInChildren<NetworkedGameState>().ToArray();
			CurrentTurnStage = TurnStage.Charleston;
			TurnPlayerIx = 1;
			ExposingPlayerIx = -1;
			numTilesExposedThisTurn = 0;
			_racks = new[] {Rack0, Rack1, Rack2, Rack3};
		}

		public void PlayerReadyToStartGamePlay()
		{
			_readyToStart++;
			if (_readyToStart == HumanPlayerCount)
			{
				TurnManagerServer.StartGame();
			}
		}

		public void SetEndGameRacks(List<List<int>> racks)
		{
			if (racks.Count != 4) throw new UnityException("SetEndGameRacks: racks does not have 4 entries");

			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				for (int i = 0; i < racks[playerIx].Count; i++)
				{
					_racks[playerIx].Set(i, racks[playerIx][i]);
				}
			}
		}
	}

	public enum TurnStage
	{
		Charleston,
		PickUp,
		Discard,
		Call,
		Expose
	}
	
	public interface IFusionManagerGlobal
	{
		public NetworkArray<PlayerRef> Players { get; }
		public int TurnPlayerIx { get; }
		public int LocalPlayerIx { get; }
		public int DiscardTileId { get; }
		public bool IsMyTurn { get; }
		public bool IsMyExpose { get; }
		public TurnManagerServer TurnManagerServer { get; set; }
		int PlayerIx(int playerId);
		int PlayerIx(PlayerRef playerRef);
		INetworkedGameState[] NetworkedGameStates { get; set; }
		int HumanPlayerCount { get; }
		public TurnStage CurrentTurnStage { get; set; }
		public int MahJonggWinner { get; }
		public void SetEndGameRacks(List<List<int>> racks);
		public List<int>[] Racks { get; }
	}
}