using System.Diagnostics;

namespace Resources
{
	public class FakeRpcS2CHandler : IRpcS2CHandler
	{
		public void RPC_S2C_SendGameState(int playerId, int requestId, CLoc[] gameState)
		{
			Debug.WriteLine($"RPC_S2C_SendGameState(playerId: {playerId}, requestId: {requestId})");
		}
	}
}