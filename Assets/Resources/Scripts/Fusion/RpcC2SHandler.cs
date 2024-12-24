using System.Diagnostics;
using Fusion;
using JetBrains.Annotations;

namespace Resources
{
	public class RpcC2SHandler : NetworkBehaviour, IRpcC2SHandler
	{
		[CanBeNull] public TileTrackerServer tileTracker;
		public FusionManagerClient fusionManagerClient;
		
		// Client requests server for game state
		/* note: the reason I'm not just making the method default info = default is so that the interface can have a no parameter version. Maybe there's a better way */
		public void RPC_C2S_RequestGameState() => RPC_C2S_RequestGameState(default);
		[Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
		public void RPC_C2S_RequestGameState(RpcInfo info)
		{
			// TODO: this assertion failed but I can't find where it logged. Should I switch to unity logging?
			Debug.Assert(tileTracker != null);
			Debug.WriteLine("RPC_C2C_RequestRack");
			tileTracker.SendGameStateToPlayer(info.Source.PlayerId);
		}
	}
	
	public interface IRpcC2SHandler
	{
		void RPC_C2S_RequestGameState();
	}
}