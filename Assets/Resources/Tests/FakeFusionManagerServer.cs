using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
	public class FakeFusionManagerServer
	{
		public int PlayerCount { get; set; }
		public INetworkedGameState[] NetworkedGameStates { get; set; } = { 
			new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
		};

		public void NextTurn()
		{
			Debug.Log("Next turn");
		}
	}
}