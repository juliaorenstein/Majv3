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
		private List<Transform> _otherPrivateRacks;
		private List<Transform> _otherCharlestonBoxes;
		private int _localPlayerIx;
		
		private UIHandlerMono _uiHandler;

		private void Start()
		{
			_charlestonTransform = GameObject.Find("Charleston Pass").transform;
			_charlestonPassArr = _charlestonTransform.GetComponent<CharlestonPassArray>();
			_uiHandler = GetComponent<UIHandlerMono>();
			_otherPrivateRacks = new()
			{
				GameObject.Find("Other Rack 1").transform.GetChild(1),
				GameObject.Find("Other Rack 2").transform.GetChild(1),
				GameObject.Find("Other Rack 3").transform.GetChild(1)
			};
			_otherCharlestonBoxes = new()
			{
				GameObject.Find("Other Charleston 1").transform,
				GameObject.Find("Other Charleston 2").transform,
				GameObject.Find("Other Charleston 3").transform
			};
		}

		public void SetLocalPlayerIx(int playerIx) => _localPlayerIx = playerIx;

		public void MoveLocalTileRackToCharlestonBox(Transform tileFace, Transform spot)
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

		public void MoveOtherTileToCharlestonBox(int playerIx, int spotIx)
		{
			int rackIx = (playerIx - _localPlayerIx + 4) % 4 - 1;
			Transform spot = _otherCharlestonBoxes[rackIx].GetChild(spotIx);
			Transform tileBack = _otherPrivateRacks[rackIx].GetChild(_otherPrivateRacks[rackIx].childCount - 1);
			tileBack.SetParent(spot);
			_lerp.StartX = tileBack.position.x;
			_lerp.StartY = tileBack.position.y;
			tileBack.position = spot.position;
			
			// lerp!
			_lerp.TileFace = tileBack.GetChild(0);
			_lerp.EndX = tileBack.position.x;
			_lerp.EndY = tileBack.position.y;
			_lerp.Active = true;
			_lerp.T = 0;
		}

		private Lerp _lerp = new Lerp { Active = false };
		
		private void Update()
		{
			_uiHandler.Lerp(ref _lerp);
		}
	}
}