using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Resources
{
	public class DragHandlerMono : MonoBehaviour
		, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		// mono and tileTracker set in SetupMono
		public Mono mono;
		public TileTrackerClient TileTracker;
		public int tileId;
		private CLoc CurLoc => TileTracker.GetTileLoc(tileId);
		private Image _image;
		
		// the position at the tile at the beginning of a drag
		private Vector3 _startPosition;

		private void Start()
		{
			_image = GetComponentInChildren<Image>();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
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
			// get list of current raycast results
			List<RaycastResult> candidates = new();
			EventSystem.current.RaycastAll(eventData, candidates);
			// there should only be up to two results: a location and possibly another tile
			Debug.Assert(candidates.Count <= 2); 
			
			// translates candidates to their CLocs. If no valid CLoc, set to pool (and fail an assertion)
			List<CLoc> candidateLocs = candidates.Select(candidate => 
				mono.TransformToLoc.GetValueOrDefault(candidate.gameObject.transform)).ToList();
			Debug.Assert(!candidateLocs.Contains(CLoc.Pool)
				, "OnEndDrag: RaycastResult doesn't correspond to CLoc.");
			
			// if this is a rack rearrange, we don't need to notify the server
			if (IsRackRearrange())
			{
				DoRackRearrange();
				return;
			}
			if (IsDiscard()) 
			{
				DoDiscard();
				return;
			}

			if (IsExpose())
			{
				DoExpose();
				return;
			}

			if (IsJokerExchange())
			{
				DoJokerExchange();
				return;
			}
				
				
			

			// if we reach this point, the tile wasn't dropped in a new location, so move the tile back to where it started.
			transform.position = _startPosition;

			bool IsRackRearrange() =>
				CurLoc == CLoc.LocalPrivateRack && candidateLocs.Contains(CLoc.LocalPrivateRack); 

			bool IsDiscard() => CurLoc == CLoc.LocalPrivateRack && candidateLocs.Contains(CLoc.Discard);
			
			bool IsExpose() => CurLoc == CLoc.LocalPrivateRack && candidateLocs.Contains(CLoc.LocalDisplayRack);

			bool IsJokerExchange() =>
				CurLoc == CLoc.LocalPrivateRack && TileTracker.DisplayRacks.Intersect(candidateLocs).Any();

			void DoRackRearrange()
			{
				throw new NotImplementedException();
			}

			void DoDiscard()
			{
				throw new NotImplementedException();
			}

			void DoExpose()
			{
				throw new NotImplementedException();
			}

			void DoJokerExchange()
			{
				throw new NotImplementedException();
			}
		}
	}
}