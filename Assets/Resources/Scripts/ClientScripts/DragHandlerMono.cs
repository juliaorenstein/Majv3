using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// TODO: extract out logic from mono and then make unit tests
namespace Resources
{
	public class DragHandlerMono : MonoBehaviour
		, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		// mono and tileTracker set in SetupMono
		public UIHandlerMono uiHandlerMono;
		public TileTrackerClient TileTracker;
		public InputSender InputSender;
		private FusionManagerGlobal _fusionManager;
		private CharlestonUIHandlerMono _charlestonUI;
		
		public int tileId;
		private CLoc CurLoc => TileTracker.GetTileLoc(tileId);
		private Image _image;
		private Transform _dragTransform;
		private Transform _tileTransform;
		private Transform _charlestonTransform;
		private readonly List<CLoc> _charlestonSpots = new() 
			{ CLoc.CharlestonSpot1, CLoc.CharlestonSpot2, CLoc.CharlestonSpot3 };

		private void Start()
		{
			_image = GetComponent<Image>();
			_dragTransform = GameObject.Find("Dragging").transform;
			_tileTransform = transform.parent;
			_fusionManager = FindObjectsByType<FusionManagerGlobal>(FindObjectsSortMode.None)[0];
			_charlestonTransform = GameObject.Find("Charleston").transform;
			_charlestonUI = GameObject.Find("GameManager").GetComponent<CharlestonUIHandlerMono>();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			// set parent to dragging GameObject so that tile remains on top of all other UI
			_image.transform.SetParent(_dragTransform, true);
			// turn off raycast target while dragging the tile
			_image.raycastTarget = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			transform.position += (Vector3)eventData.delta;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			// TODO: potential refactor. Check chatGPT "Drag Validation Workflow Design" chat for details
			
			transform.SetParent(_tileTransform, true);
			
			// get list of current raycast results
			List<RaycastResult> candidates = new();
			EventSystem.current.RaycastAll(eventData, candidates);
			
			// translates candidates to their CLocs. If no valid CLoc, set to pool
			List<CLoc> candidateLocs = candidates.Select(candidate => 
				uiHandlerMono.TransformToLoc.GetValueOrDefault(candidate.gameObject.transform)).ToList();
			
			// if this is a rack rearrange, we don't need to notify the server
			if (IsRackRearrange())
			{
				Debug.Log("Drag: Rack Rearrange");
				DoMoveToRack();
			}
			else if (IsDiscard()) 
			{
				Debug.Log("Drag: Discard");
				DoDiscard();
			}

			else if (IsExpose())
			{
				Debug.Log("Drag: Expose");
				DoExpose();
			}

			else if (IsJokerExchange(out CLoc displayRack, out int exchangeIx))
			{
				Debug.Log("Drag: Joker Exchange");
				DoJokerExchange(displayRack, exchangeIx);
			}
			
			else if (IsRackToCharleston())
			{
				Debug.Log("Drag: Rack to Charleston");
				DoRackToCharleston();
			}

			else
			{
				// if none of the above, this was not a valid drag - return tile to original place and enable raycast again.
				Debug.Log("Drag: Invalid");
				MoveBack();
			}
			
			return;

			bool IsRackRearrange() =>
				CurLoc == CLoc.LocalPrivateRack && candidateLocs.Contains(CLoc.LocalPrivateRack);

			bool IsDiscard()
			{
				if (CurLoc is not CLoc.LocalPrivateRack) return false;
				if (!candidateLocs.Contains(CLoc.Discard)) return false;
				if (_fusionManager.CurrentTurnStage is TurnStage.Discard && _fusionManager.IsMyTurn) return true;
				if (_fusionManager.CurrentTurnStage is TurnStage.Expose
				    && _fusionManager.IsMyExpose
				    && _fusionManager.numTilesExposedThisTurn > 2) return true;
				return false;
			}

			bool IsExpose() => _fusionManager.CurrentTurnStage is TurnStage.Expose
			                   && _fusionManager.IsMyExpose
			                   && CurLoc is CLoc.LocalPrivateRack
			                   && candidateLocs.Contains(CLoc.LocalDisplayRack)
			                   && Tile.AreSame(tileId, _fusionManager.DiscardTileId);

			bool IsJokerExchange(out CLoc displayRack, out int exchangeIx)
			{ // TODO: need to test this
				displayRack = default;
				exchangeIx = -1;
				if (CurLoc is not CLoc.LocalPrivateRack) return false;
				if (_fusionManager.CurrentTurnStage is TurnStage.Discard && _fusionManager.IsMyTurn
				    || _fusionManager.CurrentTurnStage is TurnStage.Expose && _fusionManager.IsMyExpose)
				{
					displayRack = TileTracker.DisplayRacks.Intersect(candidateLocs).FirstOrDefault();
					if (displayRack == default) return false;
					List<int> displayRackContents = TileTracker.GetLocContents(displayRack);
					List<int> jokerLocs = displayRackContents.FindAll(Tile.IsJoker);
					if (jokerLocs.Count == 0) return false;

					// the rack has jokers on it. Now make sure that the tile being offered is valid
					// this means that at least one joker will have the tile to the left of it be the tile we want
					// nobody exposes full joker segments, can't happen in the game
					foreach (int jokerLoc in jokerLocs)
					{
						int leftOfJoker = displayRackContents[jokerLoc - 1];
						if (Tile.AreSame(tileId, leftOfJoker, false)) return true;
					}
				}
				return false;
			}
			
			bool IsRackToCharleston() => _fusionManager.CurrentTurnStage == TurnStage.Charleston 
			                       && CurLoc is CLoc.LocalPrivateRack
                                   && candidateLocs.Intersect(_charlestonSpots).Any();

			void DoMoveToRack()
			{
				int siblingIndexOfTileDroppedOn = -1;
				int rightOfCenter = 0; // using int instead of bool for math down below
				int movingRight = 0; // same as above
				foreach (var candidate in candidates.Where(
					         candidate => candidate.gameObject.CompareTag("Tile")))
				{
					siblingIndexOfTileDroppedOn = candidate.gameObject.transform.parent.GetSiblingIndex();
					rightOfCenter = transform.position.x > candidate.gameObject.transform.position.x ? 1 : 0;
					movingRight = siblingIndexOfTileDroppedOn > _tileTransform.GetSiblingIndex() ? 1 : 0;
					// charleston band aid
					if (_tileTransform.parent.parent == _charlestonTransform) movingRight = 0;
					break;
				}
				// enable raycast again
				_image.raycastTarget = true;
				
				// if not dropped on another tile, send to end of rack
				if (siblingIndexOfTileDroppedOn == -1)
				{
					TileTracker.MoveTile(tileId, CLoc.LocalPrivateRack);
					return;
				}
				
				// otherwise, drop on the appropriate index
				int dropIx = siblingIndexOfTileDroppedOn + rightOfCenter - movingRight;
				TileTracker.MoveTile(tileId, CLoc.LocalPrivateRack, dropIx);
			}
			
			void DoDiscard()
			{
				InputSender.RequestDiscard(tileId);
				TileTracker.MoveTile(tileId, CLoc.Discard);
			}
			
			void DoExpose()
			{
				InputSender.RequestExpose(tileId);
				TileTracker.MoveTile(tileId, CLoc.LocalDisplayRack);
			}

			void DoJokerExchange(CLoc displayRack, int exchangeIx)
			{
				Debug.Log("Joker Exchange not implemeneted");
				MoveBack();
			}

			void DoRackToCharleston()
			{
				InputSender.RequestTileToCharlestonBox(tileId);
				Transform spot = uiHandlerMono.LocToTransform[candidateLocs.Intersect(_charlestonSpots).FirstOrDefault()];
				_charlestonUI.MoveTileRackToCharlestonBox(transform, spot);
			}

			void MoveBack()
			{
				uiHandlerMono.MoveTile(tileId, CLoc.LocalPrivateRack);
			}
		}
	}
}