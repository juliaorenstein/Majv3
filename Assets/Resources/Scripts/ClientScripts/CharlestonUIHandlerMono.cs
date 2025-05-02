using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Resources
{
	public class CharlestonUIHandlerMono : MonoBehaviour
	{
		// charleston spots
		private Transform _charlestonTransform;
		private CharlestonPassArray _charlestonPassArr;
		private readonly List<CLoc> _charlestonSpots = new() 
			{ CLoc.CharlestonSpot1, CLoc.CharlestonSpot2, CLoc.CharlestonSpot3 };
		
		private UIHandlerMono _uiHandler;

		private void Start()
		{
			_charlestonTransform = GameObject.Find("Charleston Pass").transform;
			_charlestonPassArr = _charlestonTransform.GetComponent<CharlestonPassArray>();
			_uiHandler = GetComponent<UIHandlerMono>();
		}

		public void MoveTileRackToCharlestonBox(Transform tileFace, Transform spot)
		{
			Transform tile = tileFace.parent;
			bool spotOccupied = spot.childCount > 0;
			
			// get the facts
			_lerp.TileFace = tileFace;
			_lerp.StartX = _lerp.TileFace.position.x;
			_lerp.StartY = _lerp.TileFace.position.y;

			// set parent to new location and lerp the face
			tile.SetParent(spot);
			tile.position = spot.position;

			_lerp.EndX = tile.position.x;
			_lerp.EndY = tile.position.y;
			_lerp.Active = true;
			
			if (spotOccupied) _uiHandler.MoveTile(spot.GetChild(0), CLoc.LocalPrivateRack);

			tileFace.GetComponent<Image>().raycastTarget = true;
		}

		private Lerp _lerp = new Lerp { Active = false };
		
		private void Update()
		{
			_uiHandler.Lerp(ref _lerp);
		}
	}
}