using System.Collections.Generic;
using Fusion;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Resources
{
	public class RpcS2CHandler : NetworkBehaviour, IRpcS2CHandler
	{
		[CanBeNull] public FusionManagerServer fusionManager;
		public TileTrackerClient tileTracker;

		public void RPC_S2C_SendGameState(int playerId, int requestId, CLoc[] gameState)
		{
			Debug.Assert(fusionManager != null);
			RPC_S2C_SendGameState(fusionManager.Players[playerId], requestId, gameState);
		}
			
		[Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
		private void RPC_S2C_SendGameState(PlayerRef playerRef, int requestId, CLoc[] gameState)
		{
			Debug.Assert(tileTracker != null);
			Debug.WriteLine($"RPC_S2C_SendGameState(playerRef: {playerRef}, requestId: {requestId})");
			tileTracker.ReceiveGameState(requestId, gameState);
		}
	}
	
	public interface IRpcS2CHandler
	{
		void RPC_S2C_SendGameState(int playerId, int requestId, CLoc[] gameState);
	}
}