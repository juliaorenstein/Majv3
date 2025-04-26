using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Resources
{
	public class Mono : MonoBehaviour, IMono
	{
		public readonly Dictionary<CLoc, Transform> LocToTransform = new();
		public readonly Dictionary<Transform, CLoc> TransformToLoc = new();
		public List<Transform> AllTileTransforms = new();

		private Button _pickUp;
		private Button _call;
		private Button _wait;
		private Button _pass;
		private Button _neverMind;
		private Dictionary<Action, Button> _actionToButton;

		private GameObject _displayRackSpace;

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
			
			_pickUp = GameObject.Find("Pick Up").GetComponent<Button>();
			_call = GameObject.Find("Call").GetComponent<Button>();
			_wait = GameObject.Find("Wait").GetComponent<Button>();
			_pass = GameObject.Find("Pass").GetComponent<Button>();
			_neverMind = GameObject.Find("Never Mind").GetComponent<Button>();
			_actionToButton = new()
			{
				{ Action.PickUp, _pickUp },
				{ Action.Call, _call },
				{ Action.Wait, _wait },
				{ Action.Pass, _pass },
				{ Action.NeverMind, _neverMind }
			};
			
			_displayRackSpace = UnityEngine.Resources.Load<GameObject>("Prefabs/Space");
		}
		
		public void MoveTile(int tileId, CLoc loc, int ix = -1)
		{
			Transform tileTransform = AllTileTransforms[tileId];
			Transform locTransform = LocToTransform[loc];
			MoveTile(tileTransform, locTransform, ix);
		}

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
			
			// if loc is the private local rack, set raycast target = True. Otherwise, false
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
			_t = 0;
			_lerping = false;
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_locTransform);
			_tileFace.position = _tileFace.parent.position;
		}
	}
	
	public interface IMono
	{
		public void MoveTile(int tileId, CLoc loc, int ix = -1);
		public void UpdatePrivateRackCount(CLoc privateRack, int count);
		public void SetActionButton(Action action, bool state);
		public void AddSpaceToDisplayRack(CLoc loc);
	}
	
	// TODO: when a player discards, other players should see the tile move from that rack
}