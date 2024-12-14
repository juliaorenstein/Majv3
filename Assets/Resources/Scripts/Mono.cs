using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources 
{
	public class Mono : MonoBehaviour, IMono
	{
		private readonly Dictionary<CLoc, Transform> _locToTransform = new();
		public List<Transform> AllTileTransforms = new();

		private void Start()
		{
			// set up dictionary
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
		}
		
		public void MoveTile(int tileId, CLoc loc, int ix = -1)
		{
			Transform tileTransform = AllTileTransforms[tileId];
			Transform locationTransform = _locToTransform[loc];
			
			tileTransform.SetParent(locationTransform);
			if (ix != -1) tileTransform.SetSiblingIndex(ix);
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)locationTransform);
		}
	}
}