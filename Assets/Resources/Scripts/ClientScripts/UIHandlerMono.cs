using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Resources
{
	public class UIHandlerMono : MonoBehaviour, IUIHandler
	{
		public readonly Dictionary<CLoc, Transform> LocToTransform = new();
		public readonly Dictionary<Transform, CLoc> TransformToLoc = new();
		public List<Transform> allTileTransforms = new();

		private Button _pickUp;
		private Button _call;
		private Button _confirm;
		private Button _cancel;
		private Dictionary<Action, Button> _actionToButton;

		private GameObject _displayRackSpace;

		//private Transform _charlestonBox;
		//private float _charlestonX;
		//private readonly float[] _charlestonY = new float[3];

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
			
			//_charlestonBox = GameObject.Find("Charleston").transform;
			//_charlestonX = LocToTransform[CLoc.OtherDisplayRack1].parent.position.x * 2;
			//_charlestonY[0] = LocToTransform[CLoc.OtherPrivateRack1].parent.position.y;
			//_charlestonY[1] = LocToTransform[CLoc.OtherPrivateRack2].parent.position.y;
			//_charlestonY[2]= LocToTransform[CLoc.OtherPrivateRack3].parent.position.y;
		}
		
		public void MoveTile(int tileId, CLoc loc, int ix = -1)
		{
			Transform tileTransform = allTileTransforms[tileId];
			MoveTile(tileTransform, loc, ix);
			
		}

		public void MoveTile(Transform tileTransform, CLoc loc, int ix = -1)
		{
			Transform locTransform = LocToTransform[loc];
			MoveTile(tileTransform, locTransform, ix);
		}

		private void MoveTile(Transform tileTransform, Transform locTransform, int ix = -1)
		{
			// get the facts
			_lerp.TileFace = tileTransform.GetChild(0);
			_lerp.StartX = _lerp.TileFace.position.x;
			_lerp.StartY = _lerp.TileFace.position.y;

			// set parent to new location and lerp the face
			tileTransform.SetParent(locTransform);
			if (ix != -1) tileTransform.SetSiblingIndex(ix);
			else tileTransform.SetSiblingIndex(tileTransform.parent.childCount - 1);
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)locTransform);
			
			_lerp.EndX = tileTransform.position.x;
			_lerp.EndY = tileTransform.position.y;
			_lerp.LocTransform = locTransform;
			_lerp.Active = true;
			
			// if loc is the private local rack or charleston, set raycast target = True. Otherwise, false
			_lerp.TileFace.GetComponent<Image>().raycastTarget = locTransform == LocToTransform[CLoc.LocalPrivateRack];
		}

		// update the number of tile backs showing on a private rack.
		public void UpdatePrivateRackCount(CLoc privateRack, int count)
		{
			// TODO: is this still needed?
			/*
			// first make sure we're getting another player's private rack
			Debug.Assert(new [] {CLoc.OtherPrivateRack1, CLoc.OtherPrivateRack2, CLoc.OtherPrivateRack3}.Contains(privateRack));
			Transform rackTransform = LocToTransform[privateRack];
			// activate the tiles in the rack up to count, deactivate the rest
			for (int i = 0; i < 14; i++) rackTransform.GetChild(i).gameObject.SetActive(count > i);
			*/
		}

		public void AddSpaceToDisplayRack(CLoc displayRack)
		{
			Instantiate(_displayRackSpace, LocToTransform[displayRack]);
		}

		public void SetActionButton(Action action, bool state) =>
			_actionToButton[action].interactable = state;

		private Lerp _lerp = new Lerp { Active = false };
		
		private void Update()
		{
			Lerp(_lerp);
		}
		
		// BUG: last tile in rack is off at start
		public static void Lerp(Lerp lerp)
		{
			if (!lerp.Active) return; // quit out immediately unless lerping is active

			// Set the position of the tileFace using the values from the Lerp struct
			lerp.TileFace.position = new Vector3(
				Mathf.Lerp(lerp.StartX, lerp.EndX, lerp.T), 
				Mathf.Lerp(lerp.StartY, lerp.EndY, lerp.T), 
				0
			);
			
			// Increase the T interpolator
			lerp.T += 0.02f;

			// Check if we're done
			if (lerp.T < 1.0f) return;

			// Final position
			lerp.TileFace.position = new Vector3(lerp.EndX, lerp.EndY);
			lerp.Active = false;
			lerp.T = 0;

			// If there is a location transform, rebuild its layout
			if (lerp.LocTransform) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)lerp.LocTransform);
		}
	}
	
	public interface IUIHandler
	{
		public void MoveTile(int tileId, CLoc loc, int ix = -1);
		public void UpdatePrivateRackCount(CLoc privateRack, int count);
		public void SetActionButton(Action action, bool state);
		public void AddSpaceToDisplayRack(CLoc loc);
	}

	public class Lerp
	{
		public bool Active;
		public Transform TileFace;
		public float StartX;
		public float StartY;
		public float EndX;
		public float EndY;
		public float T;
		public Transform LocTransform;
	}
	
	// TODO: when a player discards, other players should see the tile move from that rack
}