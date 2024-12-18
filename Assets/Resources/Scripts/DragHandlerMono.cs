using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Resources
{
	public class DragHandlerMono : MonoBehaviour
		, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public Mono mono;
		private Transform _tile;

		private void Start()
		{
			_tile = gameObject.transform;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			
		}

		public void OnDrag(PointerEventData eventData)
		{
			_tile.position += (Vector3)eventData.delta;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			List<RaycastResult> candidates = new();
			EventSystem.current.RaycastAll(eventData, candidates);
			
			foreach (RaycastResult candidate in candidates)
			{
				GameObject candidateGameObject = candidate.gameObject;
				if (candidateGameObject.TryGetComponent(out Image image) && image.raycastTarget)
				{
					// this only moves the object in the UI and heirarchy
					mono.MoveTile(_tile, candidateGameObject.transform);
					// TODO: notify the server of the move, tile tracker will update when it's been accepted
					// TODO: handle case where server doesn't respond?
				}
			}
		}
	}
}