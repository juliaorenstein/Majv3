using Fusion;
using UnityEngine;

namespace Resources
{
	// One for each client
	public class InputReceiver : NetworkBehaviour
	{
		public int playerIx; 
		private TurnManagerServer _turnManager;
		private CharlestonHandlerServer _charlestonHandler;
		private NetworkButtons _previousTurnOptions;
		private CallHandler _callHandler;
		private FusionManagerGlobal _fusionManager;
		private bool _readyFlagFlipped;

		public override void Spawned()
		{
			_previousTurnOptions = default;
			playerIx = transform.GetSiblingIndex();
		}

		public void Initialize(TurnManagerServer turnManagerServer
			, CallHandler callHandler
			, CharlestonHandlerServer charlestonHandler
			, FusionManagerGlobal fusionManager)
		{
			_turnManager = turnManagerServer;
			_callHandler = callHandler;
			_charlestonHandler = charlestonHandler;
			_fusionManager = fusionManager;
		}

		public override void FixedUpdateNetwork()
		{
			if (!Runner.IsServer) return;
			if (GetInput(out Input clientInput))
			{
				// CHARLESTON - UPDATE PASS ARRAY
				if (clientInput.Action.WasPressed(_previousTurnOptions, Action.CharlestonUpdate))
				{
					Debug.Log("Input Receiver: Charleston Update");
					int[] tilesInBox = { clientInput.Charleston1, clientInput.Charleston2, clientInput.Charleston3 };
					_charlestonHandler.ClientUpdate(playerIx, tilesInBox);
				}
			}
				
			// CHARLESTON - DO PASS
			if (clientInput.Action.WasPressed(_previousTurnOptions, Action.CharlestonPass))
			{
				Debug.Log("Input Receiver: Charleston Pass");
				_charlestonHandler.PlayerReady(playerIx);
			}
				
			// CHARLESTON - START GAMEPLAY
			if (clientInput.Action.WasPressed(_previousTurnOptions, Action.SkipCharlestons)
			    || (!_readyFlagFlipped && clientInput.StartGame)) 
			{
				Debug.Log("Input Receiver: Start Game Play");
				_readyFlagFlipped = true;
				_fusionManager.PlayerReadyToStartGamePlay();
			}
				
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
				Debug.Log("Input Receiver: Joker Swap");
				_turnManager.DoJokerSwap(playerIx, clientInput.TileId, clientInput.JokerTileId);
			}

			// CALL
			else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Call))
			{
				_callHandler.PlayersCalling.Add(playerIx);
			}

			// CONFIRM
			else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Confirm))
			{
				_callHandler.PlayersCalling.Remove(playerIx);
				_callHandler.PlayersConfirmed.Add(playerIx);
			}
				
			// CANCEL
			else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Cancel))
			{
				_callHandler.PlayersCalling.Remove(playerIx);
			}
				
			// EXPOSE
			else if (clientInput.Action.WasPressed(_previousTurnOptions, Action.Expose))
			{
				Debug.Log("Input Receiver: Exposed");
				_turnManager.DoExpose(playerIx, clientInput.TileId);
			}
			
			_previousTurnOptions = clientInput.Action;
		}
	}
}