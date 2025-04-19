using Fusion;
using UnityEngine;

namespace Resources
{
	public class FusionManagerClient : NetworkBehaviour
	{
		public int PlayerId;
		public INetworkedGameState GameState;

		public bool IsMyTurn => GameState.PlayerId == GameState.TurnPlayerId;
		
		public override void Spawned()
		{
			PlayerId = Runner.LocalPlayer.PlayerId;
		}
	}
}