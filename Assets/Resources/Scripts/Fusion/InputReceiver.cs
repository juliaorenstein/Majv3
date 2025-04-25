using System.Linq;
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
		private CallHandler _callHandler;
		

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
				_callHandler ??= GetComponentInParent<CallHandler>();
				
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
				
				// CALL
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Call))
				{
					_callHandler.PlayersThinking.Remove(playerIx);
					_callHandler.PlayersPassing.Remove(playerIx);
					_callHandler.PlayersCalling.Add(playerIx);
				}

				// WAIT
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Wait))
				{
					_callHandler.PlayersCalling.Remove(playerIx);
					_callHandler.PlayersPassing.Remove(playerIx);
					_callHandler.PlayersThinking.Add(playerIx);
				}
				
				// PASS
				else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Pass))
				{
					_callHandler.PlayersThinking.Remove(playerIx);
					_callHandler.PlayersCalling.Remove(playerIx);
					_callHandler.PlayersPassing.Add(playerIx);
				}
				
				// EXPOSE
				else if (clientInput.Action.WasReleased(_previousTurnOptions, Action.Expose))
				{
					Debug.Log("Input Receiver: Exposed");
					_turnManager.DoExpose(playerIx, clientInput.TileId);
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