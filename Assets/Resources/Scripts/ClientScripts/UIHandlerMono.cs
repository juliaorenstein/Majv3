using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Resources
{
	public class UIHandlerMono : MonoBehaviour, IUIHandler
	{
		public readonly Dictionary<CLoc, Transform> LocToTransform = new();
		public readonly Dictionary<Transform, CLoc> TransformToLoc = new();
		public List<Transform> AllTileTransforms = new();

		private Button _pickUp;
		private Button _call;
		private Button _confirm;
		private Button _cancel;
		private Dictionary<Action, Button> _actionToButton;

		private GameObject _displayRackSpace;

		private Transform _charlestonBox;
		private float _charlestonX;
		private readonly float[] _charlestonY = new float[3];

		private void Start()
		{
			// set up dictionaries
			LocToTransform[CLoc.LocalPrivateRack] = GameObject.Find("Local Rack").transform.GetChild(1);
			LocToTransform[CLoc.LocalDisplayRack] = GameObject.Find("Local Rack").transform.GetChild(0);
			LocToTransform[CLoc.OtherPrivateRack1] = GameObject.Find("Other Rack 1").transform.GetChild(1);
			LocToTransform[CLoc.OtherDisplayRack1] = GameObject.Find("Other Rack 1").transform.GetChild(0);
			LocToTransform[CLoc.OtherPrivateRack2] = GameObject.Find("Other Rack 2").transform.GetChild(1);
			LocToTransform[CLoc.OtherDisplayRack2] = GameObject.Find("Other Rack 2").transform.GetChild(0);
			LocToTransform[CLoc.OtherPrivateRack3] = GameObject.Find("Other Rack 3").transform.GetChild(1);
			LocToTransform[CLoc.OtherDisplayRack3] = GameObject.Find("Other Rack 3").transform.GetChild(0);
			LocToTransform[CLoc.Discard] = GameObject.Find("Discard").transform;
			LocToTransform[CLoc.Pool] = GameObject.Find("Pool").transform;
			LocToTransform[CLoc.CharlestonSpot1] = GameObject.Find("Charleston").transform.GetChild(0);
			LocToTransform[CLoc.CharlestonSpot2] = GameObject.Find("Charleston").transform.GetChild(1);
			LocToTransform[CLoc.CharlestonSpot3] = GameObject.Find("Charleston").transform.GetChild(2);
			
			TransformToLoc[LocToTransform[CLoc.LocalPrivateRack]] = CLoc.LocalPrivateRack;
			TransformToLoc[LocToTransform[CLoc.LocalDisplayRack]] = CLoc.LocalDisplayRack;
			TransformToLoc[LocToTransform[CLoc.OtherPrivateRack1]] = CLoc.OtherPrivateRack1;
			TransformToLoc[LocToTransform[CLoc.OtherDisplayRack1]] = CLoc.OtherDisplayRack1;
			TransformToLoc[LocToTransform[CLoc.OtherPrivateRack2]] = CLoc.OtherPrivateRack2;
			TransformToLoc[LocToTransform[CLoc.OtherDisplayRack2]] = CLoc.OtherDisplayRack2;
			TransformToLoc[LocToTransform[CLoc.OtherPrivateRack3]] = CLoc.OtherPrivateRack3;
			TransformToLoc[LocToTransform[CLoc.OtherDisplayRack3]] = CLoc.OtherDisplayRack3;
			TransformToLoc[LocToTransform[CLoc.Discard]] = CLoc.Discard;
			TransformToLoc[LocToTransform[CLoc.Pool]] = CLoc.Pool;
			TransformToLoc[LocToTransform[CLoc.CharlestonSpot1]] = CLoc.CharlestonSpot1;
			TransformToLoc[LocToTransform[CLoc.CharlestonSpot2]] = CLoc.CharlestonSpot2;
			TransformToLoc[LocToTransform[CLoc.CharlestonSpot3]] = CLoc.CharlestonSpot3;
			
			_pickUp = GameObject.Find("Pick Up").GetComponent<Button>();
			_call = GameObject.Find("Call").GetComponent<Button>();
			_confirm = GameObject.Find("Confirm").GetComponent<Button>();
			_cancel = GameObject.Find("Cancel").GetComponent<Button>();
			_actionToButton = new()
			{
				{ Action.PickUp, _pickUp },
				{ Action.Call, _call },
				{ Action.Confirm, _confirm },
				{ Action.Cancel, _cancel }
			};
			
			_displayRackSpace = UnityEngine.Resources.Load<GameObject>("Prefabs/Space");
			
			_charlestonBox = GameObject.Find("Charleston").transform;
			_charlestonX = LocToTransform[CLoc.OtherDisplayRack1].parent.position.x * 2;
			_charlestonY[0] = LocToTransform[CLoc.OtherPrivateRack1].parent.position.y;
			_charlestonY[1] = LocToTransform[CLoc.OtherPrivateRack2].parent.position.y;
			_charlestonY[2]= LocToTransform[CLoc.OtherPrivateRack3].parent.position.y;
		}
		
		public void MoveTile(int tileId, CLoc loc, int ix = -1)
		{
			Transform tileTransform = AllTileTransforms[tileId];
			Transform locTransform = LocToTransform[loc];
			MoveTile(tileTransform, locTransform, ix);
		}

		// BUG: moving tile to pool during charleston doesn't reflect on UI
		private void MoveTile(Transform tileTransform, Transform locTransform, int ix = -1)
		{
			// get the facts
			_tileFace = tileTransform.GetChild(0);
			_startX = _tileFace.position.x;
			_startY = _tileFace.position.y;

			// set parent to new location and lerp the face
			tileTransform.SetParent(locTransform);
			if (ix != -1) tileTransform.SetSiblingIndex(ix);
			else tileTransform.SetSiblingIndex(tileTransform.parent.childCount - 1);
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)locTransform);
			
			_endX = tileTransform.position.x;
			_endY = tileTransform.position.y;
			_lerping = true;
			_locTransform = locTransform;
			
			// if loc is the private local rack or charleston, set raycast target = True. Otherwise, false
			_tileFace.GetComponent<Image>().raycastTarget = locTransform == LocToTransform[CLoc.LocalPrivateRack];
		}

		// update the number of tile backs showing on a private rack.
		public void UpdatePrivateRackCount(CLoc privateRack, int count)
		{
			/* Each rack will always have 14 child tile backs. This method activates and deactivates the appropriate
				number to match the requested count */
			
			// first make sure we're getting another player's private rack
			Debug.Assert(new [] {CLoc.OtherPrivateRack1, CLoc.OtherPrivateRack2, CLoc.OtherPrivateRack3}.Contains(privateRack));
			Transform rackTransform = LocToTransform[privateRack];
			// activate the tiles in the rack up to count, deactivate the rest
			for (int i = 0; i < 14; i++) rackTransform.GetChild(i).gameObject.SetActive(count > i);
		}
		
		public void MoveTileCharlestonBox(int tileId, CLoc spot)
		{
			Transform tileTransform = AllTileTransforms[tileId];
			Transform spotTransform = LocToTransform[spot];
			
			// get the facts
			_tileFace = tileTransform.GetChild(0);
			_startX = _tileFace.position.x;
			_startY = _tileFace.position.y;

			// set parent to new location and lerp the face
			tileTransform.SetParent(spotTransform);
			tileTransform.position = spotTransform.position;
			
			_endX = tileTransform.position.x;
			_endY = tileTransform.position.y;
			_lerping = true;
			_locTransform = spotTransform;

			_tileFace.GetComponent<Image>().raycastTarget = true;
		}
		
		public void MoveCharlestonBoxOnSubmit()
		{
			int dir = 1; // TODO: update dir correctly
			int rack = 1 - dir;

			_tileFace = _charlestonBox; // not technically a tile face here, but oh well
			_startX = _charlestonBox.position.x;
			_startY = _charlestonBox.position.y;
			_endX = _charlestonX;
			_endY = _charlestonY[rack];
			_locTransform = null;
			_lerping = true;
		}

		public void MoveOtherPlayersCharlestonOnSubmit(CLoc privateRack, int numTiles)
		{
			Debug.Assert(privateRack is CLoc.OtherPrivateRack1 or CLoc.OtherPrivateRack2 or CLoc.OtherPrivateRack3);
			int dir = 1; // TODO: need to actually set this
			int rack = 1 - dir;
			Transform rackTransform = LocToTransform[privateRack];
			int numChildren = rackTransform.childCount;
			for (int i = 0; i < numTiles; i++)
			{
				Transform tileTransform = rackTransform.GetChild(numChildren - numChildren + i);
				_tileFace = tileTransform.GetChild(0);
				_startX = _tileFace.position.x;
				_startY = _tileFace.position.y;
				_endX = _charlestonX;
				_endY = _charlestonY[rack];
				_locTransform = null;
				_lerping = true;	
			}
		}

		public void AddSpaceToDisplayRack(CLoc displayRack)
		{
			Instantiate(_displayRackSpace, LocToTransform[displayRack]);
		}

		public void SetActionButton(Action action, bool state) =>
			_actionToButton[action].interactable = state;

		private Transform _tileFace;
		private bool _lerping;
		private float _startX;
		private float _startY;
		private float _endX;
		private float _endY;
		private float _t;
		private Transform _locTransform;
		
		private void Update()
		{
			if (!_lerping) return; // quit out immediately unless lerping
			
			// set position of the tileFace
			_tileFace.position = new Vector3(Mathf.Lerp(_startX, _endX, _t), Mathf.Lerp(_startY, _endY, _t), 0);

			// increase the t interpolater
			_t += 0.02f;

			// check if we're done
			if (_t < 1.0f) return;
			_tileFace.position = new(_endX, _endY);
			if (_locTransform) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_locTransform);
			
			// reset variables
			_t = 0;
			_lerping = false;
			_tileFace = null;
			_startX = 0;
			_startY = 0;
			_endX = 0;
			_endY = 0;
			_locTransform = null;
		}
	}
	
	public interface IUIHandler
	{
		public void MoveTile(int tileId, CLoc loc, int ix = -1);
		public void UpdatePrivateRackCount(CLoc privateRack, int count);
		public void SetActionButton(Action action, bool state);
		public void AddSpaceToDisplayRack(CLoc loc);
	}
	
	// TODO: when a player discards, other players should see the tile move from that rack
}