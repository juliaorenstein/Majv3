using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// TODO: extract out logic from mono and then make unit tests
namespace Resources
{
	public class DragHandlerMono : MonoBehaviour
		, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		// mono and tileTracker set in SetupMono
		public Mono mono;
		public TileTrackerClient TileTracker;
		public InputSender InputSender;
		private FusionManagerGlobal _fusionManager;
		public int tileId;
		private CLoc CurLoc => TileTracker.GetTileLoc(tileId);
		private Image _image;
		private Transform _dragTransform;
		private Transform _tileTransform;
		
		// the position at the tile at the beginning of a drag
		private Vector3 _startPosition;

		private void Start()
		{
			_image = GetComponent<Image>();
			_dragTransform = GameObject.Find("Dragging").transform;
			_tileTransform = transform.parent;
			_fusionManager = FindObjectsByType<FusionManagerGlobal>(FindObjectsSortMode.None)[0];
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			// set parent to dragging GameObject so that tile remains on top of all other UI
			_image.transform.SetParent(_dragTransform, true);
			// store start position
			_startPosition = transform.position;
			// turn off raycast target while dragging the tile
			_image.raycastTarget = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			transform.position += (Vector3)eventData.delta;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			transform.SetParent(_tileTransform, true);
			
			// get list of current raycast results
			List<RaycastResult> candidates = new();
			EventSystem.current.RaycastAll(eventData, candidates);
			// there should only be up to two results: a location and possibly another tile
			Debug.Assert(candidates.Count <= 2); 
			
			// translates candidates to their CLocs. If no valid CLoc, set to pool
			List<CLoc> candidateLocs = candidates.Select(candidate => 
				mono.TransformToLoc.GetValueOrDefault(candidate.gameObject.transform)).ToList();
			
			// if this is a rack rearrange, we don't need to notify the server
			if (IsRackRearrange())
			{
				Debug.Log("Drag: Rack Rearrange");
				DoRackRearrange();
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

			else if (IsJokerExchange())
			{
				Debug.Log("Drag: Joker Exchange");
				DoJokerExchange();
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
				if (CurLoc != CLoc.LocalPrivateRack) return false;
				if (!candidateLocs.Contains(CLoc.Discard)) return false;
				if (!_fusionManager.IsMyTurn) return false;
				return TileTracker.GetLocContents(CLoc.LocalDisplayRack).Count
					+ TileTracker.GetLocContents(CLoc.LocalPrivateRack).Count == 14;
			}
			
			bool IsExpose() => CurLoc == CLoc.LocalPrivateRack && candidateLocs.Contains(CLoc.LocalDisplayRack);

			bool IsJokerExchange() =>
				CurLoc == CLoc.LocalPrivateRack && TileTracker.DisplayRacks.Intersect(candidateLocs).Any();

			void DoRackRearrange()
			{
				int siblingIndexOfTileDroppedOn = -1;
				int rightOfCenter = 0; // using int instead of bool for math down below
				int movingRight = 0; // same as above
				foreach (RaycastResult candidate in candidates)
				{
					if (candidate.gameObject.CompareTag("Tile")) // actually the face has the tag, not the tile itself
					{
						siblingIndexOfTileDroppedOn = candidate.gameObject.transform.parent.GetSiblingIndex();
						rightOfCenter = transform.position.x > candidate.gameObject.transform.position.x ? 1 : 0;
						movingRight = siblingIndexOfTileDroppedOn > _tileTransform.GetSiblingIndex() ? 1 : 0;
						break;
					}
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
				Debug.Log("Expose not implemented");
				MoveBack();
			}

			void DoJokerExchange()
			{
				Debug.Log("Joker Exchange not implemented");
				MoveBack();
			}

			void MoveBack()
			{
				mono.MoveTile(tileId, CLoc.LocalPrivateRack);
			}
		}
	}
}