using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

namespace Resources
{
	// One for each client
	public class InputReceiver : NetworkBehaviour
	{
		public int playerIx; 
		public TurnManagerServer TurnManager;
		private NetworkButtons _previousTurnOptions;
		

		public override void Spawned()
		{
			_previousTurnOptions = default;
		}

		public override void FixedUpdateNetwork()
		{
			if (GetInput(out Input clientInput))
			{
				TurnManager ??= GetComponentInParent<FusionManagerGlobal>().TurnManagerServer;
				// DISCARD
				if (clientInput.Action.WasPressed(_previousTurnOptions, Actions.Discard))
				{
					Debug.Log("Input Receiver: Discarding");
					TurnManager.DoDiscard(playerIx, clientInput.TileId);
				}
				
				// PICK UP
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Actions.PickUp))
				{
					Debug.Log("Input Receiver: Picking up");
					TurnManager.DoPickUp(playerIx);
				}

				// JOKER SWAP
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Actions.JokerSwap))
				{
					Debug.Log("JokerSwap not implemented");
				}

				// WAIT
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Actions.Wait))
				{
					Debug.Log("Wait not implemented");
				}
				
				// PASS
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Actions.Pass))
				{
					Debug.Log("Pass not implemented");
				}
				
				// CALL
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Actions.Call))
				{
					Debug.Log("Call not implemented");
				}
				
				// NEVER MIND
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Actions.Nevermind))
				{
					Debug.Log("Nevermind not implemented");
				}
			}

			_previousTurnOptions = clientInput.Action;
		}
	}
}