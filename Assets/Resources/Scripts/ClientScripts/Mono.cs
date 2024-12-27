using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Linq;
using Debug = System.Diagnostics.Debug;

namespace Resources
{
	public class Mono : MonoBehaviour, IMono
	{
		public readonly Dictionary<CLoc, Transform> LocToTransform = new();
		public readonly Dictionary<Transform, CLoc> TransformToLoc = new();
		public List<Transform> AllTileTransforms = new();

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
		}
		
		public void MoveTile(int tileId, CLoc loc, int ix = -1)
		{
			Transform tileTransform = AllTileTransforms[tileId];
			Transform locTransform = LocToTransform[loc];
			MoveTile(tileTransform, locTransform, ix);
		}

		private void MoveTile(Transform tileTransform, Transform locTransform, int ix = -1)
		{
			// set parent to new location (not the face yet)
			tileTransform.SetParent(locTransform);
			if (ix != -1) tileTransform.SetSiblingIndex(ix);
			else tileTransform.SetSiblingIndex(tileTransform.parent.childCount - 1);
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)locTransform);
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
	}
	
	public interface IMono
	{
		public void MoveTile(int tileId, CLoc loc, int ix = -1);
		public void UpdatePrivateRackCount(CLoc privateRack, int count);
	}
}