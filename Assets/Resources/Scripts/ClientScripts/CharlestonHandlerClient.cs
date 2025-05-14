using System.Linq;
using UnityEngine;

namespace Resources
{
	public class CharlestonHandlerClient
	{
		public readonly InputSender InputSender;
		private readonly FusionManagerGlobal _fusionManager;
		private readonly CharlestonUIHandlerMono _charlestonUIHandler;
		private readonly CharlestonHandlerNetwork _charlestonHandlerNetwork;
		private bool[][] OccupiedSpots => _charlestonHandlerNetwork.OccupiedSpots;
		private readonly bool[][] _occupiedSpots;

		public CharlestonHandlerClient(InputSender inputSender, FusionManagerGlobal fusionManager
			, CharlestonHandlerNetwork charlestonHandlerNetwork, CharlestonUIHandlerMono charlestonUIHandler)
		{
			InputSender = inputSender;
			_fusionManager = fusionManager;
			_charlestonHandlerNetwork = charlestonHandlerNetwork;
			_charlestonHandlerNetwork.CharlestonHandlerClient = this;
			_charlestonUIHandler = charlestonUIHandler;
			_occupiedSpots = new[] { new bool[3], new bool[3], new bool[3], new bool[3] };
		}
		
		public void UpdateCharlestonState()
		{
			Debug.Log("Client: Updating charleston state");
			InputSender.ClearInput();
			
			// otherwise, check for other updates
			for (int playerIx = 0; playerIx < 4; playerIx++)
			{
				for (int spotIx = 0; spotIx < 3; spotIx++)
				{
					if (OccupiedSpots[playerIx][spotIx] == _occupiedSpots[playerIx][spotIx]) continue;
					_occupiedSpots[playerIx][spotIx] = OccupiedSpots[playerIx][spotIx];
					if (playerIx == _fusionManager.LocalPlayerIx)
					{
						if (_occupiedSpots[playerIx].All(b => b) || _charlestonHandlerNetwork.IsPartialPass)
						{
							_charlestonUIHandler.EnablePassButton();
						}
					}
					else
					{
						// if newly occupied, move tile to that spot
						if (OccupiedSpots[playerIx][spotIx])
							_charlestonUIHandler.MoveOtherTileToCharlestonBox(playerIx, spotIx);

						// else, move tile back to private rack
						else
						{
							_charlestonUIHandler.MoveOtherTileFromBoxToRack(playerIx, spotIx);
						}
					}
				}
			}
			_charlestonUIHandler.UpdateReadyIndicator(_charlestonHandlerNetwork.PlayersReadyNetworked.Select(b => (bool)b).ToArray());
		}
	}
}