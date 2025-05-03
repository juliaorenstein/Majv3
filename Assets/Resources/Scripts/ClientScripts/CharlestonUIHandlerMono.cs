using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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
			Lerp lerp = new Lerp();
			lerp.TileFace = tileFace;
			lerp.StartX = lerp.TileFace.position.x;
			lerp.StartY = lerp.TileFace.position.y;

			// set parent to new location and lerp the face
			tile.SetParent(spot);
			tile.position = spot.position;

			lerp.EndX = tile.position.x;
			lerp.EndY = tile.position.y;
			lerp.Active = true;
			
			_lerps.Add(lerp);

			if (spotOccupied) _uiHandler.MoveTile(spot.GetChild(0), CLoc.LocalPrivateRack);

			tileFace.GetComponent<Image>().raycastTarget = true;
		}

		public void UpdateReadyIndicator(bool[] playersReady)
		{
			// TODO: update UI to indicate that a player is ready
		}

		public void MoveOtherTileToCharlestonBox(int playerIx, int spotIx)
		{
			int rackIx = (playerIx - _localPlayerIx + 4) % 4 - 1;
			Transform spot = _otherCharlestonBoxes[rackIx].GetChild(spotIx);
			Transform tileBack = _otherPrivateRacks[rackIx].GetChild(_otherPrivateRacks[rackIx].childCount - 1);
			
			CreateAndAddLerp(tileBack, spot);
		}

		public void DoPass()
		{
			for (int i = 0; i < _otherCharlestonBoxes.Count; i++) // Loop through each Charleston box
			{
				for (int j = 0; j < _otherCharlestonBoxes[i].childCount; j++) // Loop through each spot in the box
				{
					Transform spot = _otherCharlestonBoxes[i].GetChild(j);

					if (spot.childCount == 0) continue; // Check if the spot has a child (tile)
					
					Transform tile = spot.GetChild(0); // Get the child tile
					Transform targetRack = _otherPrivateRacks[i]; // Corresponding other private rack
					
					CreateAndAddLerp(tile, targetRack, true); // Create and add the lerp
				}
			}
		}

		private readonly List<Lerp> _lerps = new();

		private void Update()
		{
			for (int i = _lerps.Count - 1; i >= 0; i--) // going backwards so we can remove items as we go
			{
				UIHandlerMono.Lerp(_lerps[i]);
				if (!_lerps[i].Active) _lerps.RemoveAt(i);
			}
		}
		
		private void CreateAndAddLerp(Transform tile, Transform target, bool rack = false)
		{
			// Set up the lerp information
			Lerp lerp = new()
			{
				TileFace = tile.GetChild(0),
				StartX = tile.position.x,
				StartY = tile.position.y,
				EndX = target.position.x,
				EndY = target.position.y,
				Active = true,
				T = 0
			};
			tile.SetParent(target);
			
			// snap tile to position
			if (rack) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)target); // Force rebuild the layout;
			else tile.position = target.position;
			
			// make sure tile face doesn't flash to new location
			lerp.TileFace.position = new Vector3(lerp.StartX, lerp.StartY, 0);
			
			_lerps.Add(lerp);
		}
	}
}