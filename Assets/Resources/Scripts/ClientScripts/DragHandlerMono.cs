using System;
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
		public UIHandlerMono uiHandler;
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
		private Transform _localPrivateRackTransform;
		private readonly List<CLoc> _charlestonSpots = new() 
			{ CLoc.CharlestonSpot1, CLoc.CharlestonSpot2, CLoc.CharlestonSpot3 };
		

		private void Start()
		{
			_image = GetComponent<Image>();
			_dragTransform = GameObject.Find("Dragging").transform;
			_tileTransform = transform.parent;
			_localPrivateRackTransform = GameObject.Find("Local Rack").transform.GetChild(1);
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
				uiHandler.TransformToLoc.GetValueOrDefault(candidate.gameObject.transform)).ToList();

			if (IsCharlestonMove())
			{
				Debug.Log("Drag: Charleston Move");
				DoCharlestonMove();
			}
			
			// if this is a rack rearrange, we don't need to notify the server
			else if (IsRackRearrange())
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

			bool IsCharlestonMove() => _fusionManager.CurrentTurnStage is TurnStage.Charleston
			                           && CurLoc is CLoc.LocalPrivateRack // this is true even if tile comes from box
			                           && (candidateLocs.Intersect(_charlestonSpots).Any() // sending to box
			                               || _tileTransform.parent.parent == _charlestonTransform) // or FROM box
			                           && !Tile.IsJoker(tileId); // can't send joker to box
			
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
					break;
				}
				// enable raycast again
				_image.raycastTarget = true;
				
				// if not dropped on another tile, it's between two tiles or at the end of rack. 
				if (siblingIndexOfTileDroppedOn == -1)
				{
					// find the two closest tiles
					int closestIx = -1;
					float closestDist = float.MaxValue;
					int secondClosestIx = -1;
					float secondClosestDist = float.MaxValue;
					
					for (int i = 0; i < _localPrivateRackTransform.childCount; i++)
					{
						float dist = _localPrivateRackTransform.GetChild(i).position.x - transform.position.x;
						if (Math.Abs(dist) < Math.Abs(closestDist))
						{
							secondClosestIx = closestIx;
							secondClosestDist = closestDist;
							
							closestIx = i;
							closestDist = dist;
						}
						else if (Math.Abs(dist) < Math.Abs(secondClosestDist))
						{
							secondClosestIx = i;
							secondClosestDist = dist;
						}
						else break; // if distance is getting bigger we've already found the two closest
					}
					if (closestDist < 0 && secondClosestDist < 0)
					{
						siblingIndexOfTileDroppedOn = closestIx + 1;
					}
					else siblingIndexOfTileDroppedOn = Math.Max(closestIx, secondClosestIx);
				}
				
				movingRight = siblingIndexOfTileDroppedOn > _tileTransform.GetSiblingIndex() ? 1 : 0;
				// charleston band aid
				if (_tileTransform.parent.parent == _charlestonTransform) movingRight = 0;
				
				// otherwise, drop on the appropriate index
				int dropIx = siblingIndexOfTileDroppedOn + rightOfCenter - movingRight;
				uiHandler.MoveTile(tileId, CLoc.LocalPrivateRack, dropIx);
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

			void DoCharlestonMove()
			{
				// first move the tile to box if applicable
				CLoc loc = candidateLocs.Intersect(_charlestonSpots).FirstOrDefault(); // set to charleston spot if approp
				if (loc == default)
				{
					// if not a charleston spot, move to private rack
					loc = CLoc.LocalPrivateRack;
				}
				else
				{
					// if a charleston spot, see if there's another tile there and move that tile back to rack
					Transform spot = uiHandler.LocToTransform[loc];
					if (spot.childCount > 0)
					{
						uiHandler.MoveTile(spot.GetChild(0), CLoc.LocalPrivateRack);
					}
				}
				
				uiHandler.MoveTile(_tileTransform, loc);
				
				// now update server with the whole pass array			
				int[] tilesInCharleston = { -1, -1, -1 };
				for (int spotIx = 0; spotIx < 3; spotIx++)
				{
					Transform spot = _charlestonTransform.GetChild(spotIx);
					if (spot.childCount <= 0) continue;
					// confirm there is only 1 tile in the spot
					Debug.Assert(spot.childCount == 1, $"Charleston spot {spotIx} has more than one tile in it.");

					tilesInCharleston[spotIx] = spot.GetComponentInChildren<DragHandlerMono>().tileId;
				}
				InputSender.RequestCharlestonUpdate(tilesInCharleston);
			}

			void MoveBack()
			{
				uiHandler.MoveTile(_tileTransform, _tileTransform.parent, _tileTransform.GetSiblingIndex());
			}
		}
	}
}