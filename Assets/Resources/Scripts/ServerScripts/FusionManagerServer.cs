using System.Collections.Generic;
using Fusion;

namespace Resources
{
	public class FusionManagerServer : NetworkBehaviour, IFusionManagerServer
	{
		public readonly Dictionary<int, PlayerRef> Players = new();
		public INetworkedGameState[] NetworkedGameStates { get; set; }
		public int PlayerCount => Players.Count;
	}

	public interface IFusionManagerServer
	{
		public int PlayerCount { get; }
		public INetworkedGameState[] NetworkedGameStates { get; set; }
	}
}