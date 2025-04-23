using System.Linq;
using Fusion;
using UnityEngine;

namespace Resources
{
	public class FusionManagerClient : NetworkBehaviour
	{
		public int PlayerId;
		public NetworkedGameState GameState;

		public bool IsMyTurn => GameState.PlayerId == GameState.TurnPlayerId;
		
		public override void Spawned()
		{
			PlayerId = Runner.LocalPlayer.PlayerId;
			// Set up networked game state
			NetworkedGameState[] networkedGameStates = GetComponentsInChildren<NetworkedGameState>();
			NetworkedGameState networkedGameState = 
				networkedGameStates.FirstOrDefault(candidate => candidate.PlayerId == PlayerId);

			if (networkedGameState == null)
			{
				Debug.LogError($"Could not find networked game state for player {PlayerId}");
			}
		}
	}
}