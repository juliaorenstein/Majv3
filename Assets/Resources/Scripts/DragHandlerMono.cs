using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Diagnostics;
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
		
		// the position at the tile at the beginning of a drag
		private Vector3 _startPosition;

		public void OnBeginDrag(PointerEventData eventData)
		{
			// store start position
			_startPosition = transform.position;
			// turn off raycast target while dragging the tile
			GetComponentInChildren<Image>().raycastTarget = false;
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
			
			// loop through candidates and see if we can drop
			foreach (RaycastResult candidate in candidates)
			{
				// get the location this transform represents (or continue if it's a tile / something else)
				if (!mono.TransformToLoc.TryGetValue(candidate.gameObject.transform, out CLoc newLoc)) continue;
				
				// if this is a rack rearrange, we don't need to notify the server
				if (RackRearrange(newLoc))
				{
					// TODO: get new ix for the tile based on position and other tile in the candidates list
					TileTracker.MoveTile(tileId, newLoc);
					return;
				}
				// any other move, request it from the server
				TileTracker.RequestMove(tileId, newLoc);
				// TODO: notify the server of the move, tile tracker will update when it's been accepted
				// TODO: handle case where server doesn't respond?
				// TODO: re-enable raycast if appropriate
				
			}

			// if we reach this point, the tile wasn't dropped in a new location, so move the tile back to where it started.
			transform.position = _startPosition;
			
			// returns true if the proposed move is from and to the local player's rack
			// this is the only move that doesn't require communication with the server
			bool RackRearrange(CLoc newLoc) => 
				TileTracker.GetTileLoc(tileId) == CLoc.LocalPrivateRack 
				&& newLoc == CLoc.LocalPrivateRack;
		}
	}
}