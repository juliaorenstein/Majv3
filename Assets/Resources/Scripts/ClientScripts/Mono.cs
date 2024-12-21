using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts.ClientScripts 
{
	public class Mono : MonoBehaviour, IMono
	{
		readonly Dictionary<CLoc, Transform> _locToTransform = new();
		public readonly Dictionary<Transform, CLoc> TransformToLoc = new();
		public List<Transform> AllTileTransforms = new();

		private void Start()
		{
			// set up dictionaries
			_locToTransform[CLoc.LocalPrivateRack] = GameObject.Find("Local Rack").transform.GetChild(1);
			_locToTransform[CLoc.LocalDisplayRack] = GameObject.Find("Local Rack").transform.GetChild(0);
			_locToTransform[CLoc.OtherPrivateRack1] = GameObject.Find("Other Rack 1").transform.GetChild(1);
			_locToTransform[CLoc.OtherDisplayRack1] = GameObject.Find("Other Rack 1").transform.GetChild(0);
			_locToTransform[CLoc.OtherPrivateRack2] = GameObject.Find("Other Rack 2").transform.GetChild(1);
			_locToTransform[CLoc.OtherDisplayRack2] = GameObject.Find("Other Rack 2").transform.GetChild(0);
			_locToTransform[CLoc.OtherPrivateRack3] = GameObject.Find("Other Rack 3").transform.GetChild(1);
			_locToTransform[CLoc.OtherDisplayRack3] = GameObject.Find("Other Rack 3").transform.GetChild(0);
			_locToTransform[CLoc.Discard] = GameObject.Find("Discard").transform;
			_locToTransform[CLoc.Pool] = GameObject.Find("Pool").transform;
			
			TransformToLoc[_locToTransform[CLoc.LocalPrivateRack]] = CLoc.LocalPrivateRack;
			TransformToLoc[_locToTransform[CLoc.LocalDisplayRack]] = CLoc.LocalDisplayRack;
			TransformToLoc[_locToTransform[CLoc.OtherPrivateRack1]] = CLoc.OtherPrivateRack1;
			TransformToLoc[_locToTransform[CLoc.OtherDisplayRack1]] = CLoc.OtherDisplayRack1;
			TransformToLoc[_locToTransform[CLoc.OtherPrivateRack2]] = CLoc.OtherPrivateRack2;
			TransformToLoc[_locToTransform[CLoc.OtherDisplayRack2]] = CLoc.OtherDisplayRack2;
			TransformToLoc[_locToTransform[CLoc.OtherPrivateRack3]] = CLoc.OtherPrivateRack3;
			TransformToLoc[_locToTransform[CLoc.OtherDisplayRack3]] = CLoc.OtherDisplayRack3;
			TransformToLoc[_locToTransform[CLoc.Discard]] = CLoc.Discard;
			TransformToLoc[_locToTransform[CLoc.Pool]] = CLoc.Pool;
		}
		
		public void MoveTile(int tileId, CLoc loc, int ix = -1)
		{
			Transform tileTransform = AllTileTransforms[tileId];
			Transform locTransform = _locToTransform[loc];
			MoveTile(tileTransform, locTransform, ix);
		}

		public void MoveTile(Transform tileTransform, Transform locTransform, int ix = -1)
		{
			tileTransform.SetParent(locTransform);
			if (ix != -1) tileTransform.SetSiblingIndex(ix);
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)locTransform);
		}
	}
}