using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources
{
	public class DoubleClickHandler : MonoBehaviour, IPointerClickHandler
	{
		private InputSender _inputSender;
		private FusionManagerGlobal _fusionManager;
		private TileTrackerClient _tileTracker;
		private IUIHandler _uiHandler;
		private CharlestonHandlerNetwork _charlestonNetwork;
		private CharlestonUIHandlerMono _charlestonUI;
		private int _tileId;
		private CLoc CurLoc => _tileTracker.GetTileLoc(_tileId);

		public void SetFields(
			InputSender inputSender
			, FusionManagerGlobal fusionManager
			, TileTrackerClient tileTracker
			, IUIHandler uiHandler
			, CharlestonHandlerNetwork charlestonNetwork
			, CharlestonUIHandlerMono charlestonUI)
		{
			_inputSender = inputSender;
			_fusionManager = fusionManager;
			_tileTracker = tileTracker;
			_uiHandler = uiHandler;
			_charlestonNetwork = charlestonNetwork;
			_charlestonUI = charlestonUI;
			_tileId = GetComponent<DragHandlerMono>().tileId;
		}
		
		public void OnPointerClick(PointerEventData eventData)
		{
			// was this a double click?
			if (eventData.clickCount < 2) return;
			
			// charleston
			if (_fusionManager.CurrentTurnStage is TurnStage.Charleston)
			{
				switch (CurLoc)
				{
					// move from rack to box
					case CLoc.LocalPrivateRack:
					{
						// find first empty spot, send to last spot if all are occupied
						int spotIx = _charlestonNetwork.OccupiedSpots[_fusionManager.LocalPlayerIx].ToList().IndexOf(false);
						spotIx = spotIx == -1 ? 2 : spotIx;
						_charlestonUI.MoveLocalTileRackToCharlestonBox(transform, spotIx);
						//_inputSender.RequestCharlestonUpdate(_tileId, spotIx); TODO: double-click functionality
						return;
					}
					// move from box to rack
					case CLoc.CharlestonSpot1 or CLoc.CharlestonSpot2 or CLoc.CharlestonSpot3:
						_tileTracker.MoveTile(_tileId, CLoc.LocalPrivateRack);
						break;
				}
			}
			
		}
	}
}