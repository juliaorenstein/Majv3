using System.Collections.Generic;
using Fusion;

namespace Resources
{
	public class FusionManagerServer : NetworkBehaviour, IFusionManagerServer
	{
		public Dictionary<int, PlayerRef> Players = new();
		public int PlayerCount => Players.Count;
	}

	public interface IFusionManagerServer
	{
		public int PlayerCount { get; }
	}
}