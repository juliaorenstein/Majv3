using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Resources
{
	public class FakeFusionManagerGlobal : IFusionManagerGlobal
	{
		public int PlayerCount { get; set; }
		public TurnStage CurrentTurnStage { get; }
		public NetworkArray<PlayerRef> Players { get; set; } = new();
		public int TurnPlayerIx { get; set; }
		public int LocalPlayerIx => 0;
		public bool IsMyTurn => true;
		public int PlayerIx(int playerId) => 0;
		public int PlayerIx(PlayerRef playerRef) => 0;

		public INetworkedGameState[] NetworkedGameStates { get; set; } = { 
			new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
		};

		public void NextTurn()
		{
		}
	}
}