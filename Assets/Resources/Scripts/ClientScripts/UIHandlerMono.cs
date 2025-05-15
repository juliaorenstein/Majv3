using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
		private GameObject _mahJongg;

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
			_mahJongg = GameObject.Find("Mah Jongg");
		}
		
		public void MoveTile(int tileId, CLoc loc, int ix = -1)
		{
			Transform tileTransform = allTileTransforms[tileId];
			MoveTile(tileTransform, loc, ix);
			
		}

		public void MoveTile(Transform tileTransform, CLoc loc, int ix = -1, Transform showTileOnComplete = default)
		{
			Transform locTransform = LocToTransform[loc];
			MoveTile(tileTransform, locTransform, ix, showTileOnComplete);
		}

		// BUG: tile lerps to the a spot a little to the left and then snaps to the right spot
		public void MoveTile(Transform tileTransform, Transform locTransform, int ix = -1, Transform showTileOnComplete = default)
		{
			Lerp lerp = new();
			
			// get the facts
			lerp.TileFace = tileTransform.GetChild(0);
			lerp.StartX = lerp.TileFace.position.x;
			lerp.StartY = lerp.TileFace.position.y;

			// set parent to new location and lerp the face
			tileTransform.SetParent(locTransform);
			if (ix != -1) tileTransform.SetSiblingIndex(ix);
			tileTransform.position = locTransform.position; // for non layout built things.
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)locTransform);
			
			lerp.EndX = tileTransform.position.x;
			lerp.EndY = tileTransform.position.y;
			lerp.LocTransform = locTransform;
			lerp.ShowTileOnComplete = showTileOnComplete;
			lerp.Active = true;
			
			// if loc is the private local rack or charleston, set raycast target = True. Otherwise, false
			lerp.TileFace.GetComponent<Image>().raycastTarget
				= locTransform == LocToTransform[CLoc.LocalPrivateRack] 
				  || locTransform == LocToTransform[CLoc.CharlestonSpot1]
				  || locTransform == LocToTransform[CLoc.CharlestonSpot2]
				  || locTransform == LocToTransform[CLoc.CharlestonSpot3];
			
			_lerps.Add(lerp);
		}
		
		public void ExposeTile(int tileId, CLoc displayRack)
		{
			// get the private rack that corresponds to the given display rack
            CLoc privateRack = displayRack switch
            {
                CLoc.LocalDisplayRack => CLoc.LocalPrivateRack,
                CLoc.OtherDisplayRack1 => CLoc.OtherPrivateRack1,
                CLoc.OtherDisplayRack2 => CLoc.OtherPrivateRack2,
                CLoc.OtherDisplayRack3 => CLoc.OtherPrivateRack3,
                _ => throw new UnityException("UIHandlerMono.ExposeTile: displayRack isn't a valid display rack.")
            };

            // Move the tile back to the display rack
            Transform privateRackTransform = LocToTransform[privateRack];
            Transform tileBack = privateRackTransform.GetChild(privateRackTransform.childCount - 1);
            Transform tile = allTileTransforms[tileId];
            MoveTile(tileBack, displayRack, showTileOnComplete: tile);
		}

		public void AddSpaceToDisplayRack(CLoc displayRack)
		{
			Instantiate(_displayRackSpace, LocToTransform[displayRack]);
		}

		public void SetActionButton(Action action, bool state) =>
			_actionToButton[action].interactable = state;

		public void EndGame(int winnerIx, List<int>[] racks)
		{
			// validation checks
			if (winnerIx is < 0 or > 4) throw new UnityException($"UIHandlerMono.EndGame: Invalid winnerIx: {winnerIx}");
			if (racks.Length != 4) throw new UnityException($"UIHandlerMono.EndGame: Invalid racks.Length: {racks.Length}");
			
			// move all tile backs to display racks
			
		}
		
		private void Update()
		{
			for (int i = _lerps.Count - 1; i >= 0; i--) // going backwards so we can remove items as we go
			{
				Lerp(_lerps[i]);
				if (_lerps[i].Active) continue;
				_lerps.RemoveAt(i);
			}
		}

		private readonly List<Lerp> _lerps = new();
		private const float Speed = 4f;

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
			lerp.T += Time.deltaTime * Speed;

			// Check if we're done, quit out here if not
			if (lerp.T < 1.0f ) return;

			// Final position
			lerp.Active = false;
			lerp.T = 0;

			// If there is a location transform, rebuild its layout
			if (lerp.LocTransform) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)lerp.LocTransform);
			lerp.TileFace.position = lerp.TileFace.parent.position;
			
			// flip tile to show face if needed
			if (lerp.ShowTileOnComplete == default) return;
            Transform tile = lerp.ShowTileOnComplete;
            Vector3 position = lerp.TileFace.position;
            Destroy(lerp.TileFace.parent.gameObject);
            tile.SetParent(lerp.LocTransform);
            tile.position = position;
            tile.GetChild(0).position = position;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)lerp.LocTransform);
		}
	}
	
	public interface IUIHandler
	{
		public void MoveTile(int tileId, CLoc loc, int ix = -1);
		public void ExposeTile(int tileId, CLoc displayRack);
		public void SetActionButton(Action action, bool state);
		public void AddSpaceToDisplayRack(CLoc loc);
		public void EndGame(int winnerIx, List<int>[] racks);
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
		public Transform ShowTileOnComplete;
	}
	
	// TODO: when a player discards, other players should see the tile move from that rack
}