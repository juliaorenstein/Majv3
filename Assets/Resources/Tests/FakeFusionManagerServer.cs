using System.Collections.Generic;

namespace Resources
{
	public class FakeFusionManagerServer : IFusionManagerServer
	{
		public int PlayerCount { get; set; }
		public INetworkedGameState[] NetworkedGameStates { get; set; } = { 
			new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
			, new FakeNetworkedGameState()
		};
	}
}