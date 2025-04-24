using System.Linq;
using Fusion;
using UnityEngine;

namespace Resources
{
	public class FusionManagerClient : NetworkBehaviour
	{
		public PlayerRef Player;
		public NetworkedGameState GameState;

		public bool IsMyTurn => GameState.Player == GameState.TurnPlayer;

		public override void Spawned()
		{
			Player = Runner.LocalPlayer;
		}
	}
}