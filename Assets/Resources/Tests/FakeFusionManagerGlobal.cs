using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Resources
{
	public class FakeFusionManagerGlobal : IFusionManagerGlobal
	{
		public int PlayerCount { get; set; }
		public Dictionary<int, PlayerRef> Players { get; } = new();
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