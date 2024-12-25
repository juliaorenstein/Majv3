using System.Collections.Generic;

namespace Resources
{
	public class FakeFusionManagerServer : IFusionManagerServer
	{
		public int PlayerCount { get; set; }
		public NetworkedGameState[] NetworkedGameStates { get; set; }
	}
}