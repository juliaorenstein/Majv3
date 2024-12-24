using System.Diagnostics;

namespace Resources
{
	public class FakeRpcC2SHandler : IRpcC2SHandler
	{
		public FusionManagerClient FusionManagerClient { get; set; }

		public void RPC_C2S_RequestGameState()
		{
			Debug.WriteLine("RPC_C2S_RequestGameState");
		}
	}
}