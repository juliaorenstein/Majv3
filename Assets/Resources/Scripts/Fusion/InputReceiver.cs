using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

namespace Resources
{
	// One for each client
	public class InputReceiver : NetworkBehaviour
	{
		public int playerIx; 
		private TurnManagerServer _turnManager;
		private NetworkButtons _previousTurnOptions;
		

		public override void Spawned()
		{
			_previousTurnOptions = default;
			playerIx = transform.GetSiblingIndex();
		}

		public override void FixedUpdateNetwork()
		{
			if (!Runner.IsServer) return;
			if (GetInput(out Input clientInput))
			{
				_turnManager ??= GetComponentInParent<FusionManagerGlobal>().TurnManagerServer;
				// DISCARD
				if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Discard))
				{
					Debug.Log("Input Receiver: Discarding");
					_turnManager.DoDiscard(playerIx, clientInput.TileId);
				}
				
				// PICK UP
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.PickUp))
				{
					Debug.Log("Input Receiver: Picking up");
					_turnManager.DoPickUp(playerIx);
				}

				// JOKER SWAP
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.JokerSwap))
				{
					Debug.Log("JokerSwap not implemented");
				}

				// WAIT
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Wait))
				{
					Debug.Log("Wait not implemented");
				}
				
				// PASS
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Pass))
				{
					Debug.Log("Pass not implemented");
				}
				
				// CALL
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Call))
				{
					Debug.Log("Call not implemented");
				}
				
				// NEVER MIND
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.NeverMind))
				{
					Debug.Log("NeverMind not implemented");
				}
			}

			_previousTurnOptions = clientInput.Action;
		}
	}
}