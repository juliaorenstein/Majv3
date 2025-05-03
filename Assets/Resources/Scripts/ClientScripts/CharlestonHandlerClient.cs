using System.Linq;
using UnityEngine;

namespace Resources
{
	public class CharlestonHandlerClient
	{
		private readonly InputSender _inputSender;
		private readonly FusionManagerGlobal _fusionManager; // TODO: need to initialize
		private readonly CharlestonUIHandlerMono _charlestonUIHandler;
		private readonly CharlestonHandlerNetwork _charlestonHandlerNetwork;
		private bool[][] OccupiedSpots => _charlestonHandlerNetwork.OccupiedSpots;
		private readonly bool[][] _occupiedSpots;

		public CharlestonHandlerClient(InputSender inputSender, FusionManagerGlobal fusionManager
			, CharlestonHandlerNetwork charlestonHandlerNetwork, CharlestonUIHandlerMono charlestonUIHandler)
		{
			_inputSender = inputSender;
			_fusionManager = fusionManager;
			_charlestonHandlerNetwork = charlestonHandlerNetwork;
			_charlestonHandlerNetwork.CharlestonHandlerClient = this;
			_charlestonUIHandler = charlestonUIHandler;
			_occupiedSpots = new[] { new bool[3], new bool[3], new bool[3], new bool[3] };
		}
		
		public void UpdateCharlestonState()
		{
			Debug.Log("Client: Updating charleston state");
			_inputSender.ClearInput();

			// if everybody is ready, move tiles to other private racks
			if (_charlestonHandlerNetwork.PlayersReady.All(b => b))
			{
				int dir = _charlestonHandlerNetwork.PassDir[_charlestonHandlerNetwork.PassNum - 1];
				_charlestonUIHandler.DoPass(dir);
				return;
			}
			
			// otherwise, check for other updates
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				// already updated UI locally for local player changes
				if (playerIx == _fusionManager.LocalPlayerIx) continue;
				
				for (int spotIx = 0; spotIx < 3; spotIx++)
				{
					if (OccupiedSpots[playerIx][spotIx] == _occupiedSpots[playerIx][spotIx]) continue;
					_occupiedSpots[playerIx][spotIx] = OccupiedSpots[playerIx][spotIx];
					
					// if newly occupied, move tile to that spot
					if (OccupiedSpots[playerIx][spotIx])
						_charlestonUIHandler.MoveOtherTileToCharlestonBox(playerIx, spotIx);

					// else, move tile back to private rack
					// TODO: move tile back to rack
				}
			}
			_charlestonUIHandler.UpdateReadyIndicator(_charlestonHandlerNetwork.PlayersReady.Select(b => (bool)b).ToArray());
		}
	}
}