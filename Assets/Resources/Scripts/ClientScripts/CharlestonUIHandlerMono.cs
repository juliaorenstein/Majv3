using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace Resources
{
	public class CharlestonUIHandlerMono : MonoBehaviour
	{
		private List<Transform> _privateRacks;
		private List<Transform> _charlestonBoxes;
		private int _localPlayerIx;
		private Transform _tileBack;
		private Transform _pool;
		

		private UIHandlerMono _uiHandler;
		private TileTrackerClient _tileTracker;
		private CharlestonHandlerNetwork _charlestonNetwork;
		
		private Button _passButton;
		private TextMeshProUGUI _passButtonText;

		private void Start()
		{
			_uiHandler = GetComponent<UIHandlerMono>();
			_privateRacks = new()
			{
				GameObject.Find("Local Rack").transform.GetChild(1),
				GameObject.Find("Other Rack 1").transform.GetChild(1),
				GameObject.Find("Other Rack 2").transform.GetChild(1),
				GameObject.Find("Other Rack 3").transform.GetChild(1)
			};
			_charlestonBoxes = new()
			{
				GameObject.Find("Charleston").transform,
				GameObject.Find("Other Charleston 1").transform,
				GameObject.Find("Other Charleston 2").transform,
				GameObject.Find("Other Charleston 3").transform
			};
			_tileBack = ((GameObject)UnityEngine.Resources.Load("Prefabs/Tile Back")).transform;
			_pool = GameObject.Find("Pool").transform;
			
			_passButton = GameObject.Find("Charleston Pass").GetComponent<Button>();
			_passButtonText = _passButton.GetComponentInChildren<TextMeshProUGUI>();
		}

		public void SetTileTrackerAndCharlestonNetwork(TileTrackerClient tileTracker, CharlestonHandlerNetwork charlestonNetwork)
		{
			_tileTracker = tileTracker;
			_charlestonNetwork = charlestonNetwork;
		}

		public void SetLocalPlayerIx(int playerIx) => _localPlayerIx = playerIx;

		public void MoveLocalTileRackToCharlestonBox(Transform tileFace, Transform spot)
		{
			Transform tile = tileFace.parent;
			bool spotOccupied = spot.childCount > 0;

			// get the facts
			Lerp lerp = new Lerp { TileFace = tileFace };
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
		}

		public void UpdateReadyIndicator(bool[] playersReady)
		{
			// TODO: update UI to indicate that a player is ready
		}

		public void MoveOtherTileToCharlestonBox(int playerIx, int spotIx)
		{
			int rackIx = (playerIx - _localPlayerIx + 4) % 4;
			Transform spot = _charlestonBoxes[rackIx].GetChild(spotIx);
			Transform tileBack = _privateRacks[rackIx].GetChild(_privateRacks[rackIx].childCount - 1);
			
			CreateAndAddLerp(tileBack, spot);
		}

		public void EnablePassButton()
		{
			_passButton.interactable = true;
		}

		public void DoPass(int dir, int nextDir)
		{
			Debug.Log("UI: doing pass");
			
			for (int i = 0; i < _charlestonBoxes.Count; i++) // Loop through each Charleston box
			{
				for (int j = 0; j < _charlestonBoxes[i].childCount; j++) // Loop through each spot in the box
				{
					Transform spot = _charlestonBoxes[i].GetChild(j);

					if (spot.childCount == 0) continue; // Check if the spot has a child (tile)
					
					Transform tile = spot.GetChild(0); // Get the child tile
					int targetIx = (i + 2 - dir) % 4;
					
					Transform targetRack = _privateRacks[targetIx]; // Corresponding other private rack
					
					CreateAndAddLerp(tile, targetRack, true); // Create and add the lerp
					if (i == 0) _passFromLocalIxs.Add(tile); // track tiles FROM local rack to flip them later
					if (targetIx == 0) _passToLocalIxs.Add(tile); // track tiles TO local rack to flip them later
				}
			}
			
			string text = nextDir switch
			{
				1 => "Pass Right",
				0 => "Pass Across",
				-1 => "Pass Left",
				_ => "Invalid Directions"
			};
			_passButtonText.SetText(text);
		}

		private readonly List<Lerp> _lerps = new();
		private readonly List<Transform> _passFromLocalIxs = new();
		private readonly List<Transform> _passToLocalIxs = new();

		private void Update()
		{
			for (int i = _lerps.Count - 1; i >= 0; i--) // going backwards so we can remove items as we go
			{
				UIHandlerMono.Lerp(_lerps[i]);
				if (_lerps[i].Active) continue;
				// if done lerping this tile, do some final checks then remove from list
				_lerps[i].TileFace.GetComponent<Image>().raycastTarget = true;
				if (_passFromLocalIxs.Contains(_lerps[i].TileFace.parent)) ReplaceTileFaceWithTileBack(_lerps[i].TileFace.parent);
				// BUG: next line throwing an error on partial passes
				else if (_passToLocalIxs.Contains(_lerps[i].TileFace.parent)) ReplaceTileBackWithTileFace(_lerps[i].TileFace.parent);
				_lerps.RemoveAt(i);
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
				Active = true,
				T = 0
			};
			tile.SetParent(target);
			
			// snap tile to position
			if (rack) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)target); // Force rebuild the layout;
			else tile.position = target.position;
			lerp.EndX = tile.position.x;
			lerp.EndY = tile.position.y;
			
			// make sure tile face doesn't flash to new location
			lerp.TileFace.position = new Vector3(lerp.StartX, lerp.StartY, 0);
			
			_lerps.Add(lerp);
		}

		
		private void ReplaceTileFaceWithTileBack(Transform tileTransform)
		{
			Transform rack = tileTransform.parent;
			int siblingIx = tileTransform.GetSiblingIndex();
			Instantiate(_tileBack, rack).SetSiblingIndex(siblingIx);
			tileTransform.SetParent(_pool);
			tileTransform.position = _pool.position;
			
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rack);
		}
		
		private void ReplaceTileBackWithTileFace(Transform tileBack)
		{
			Transform rack = tileBack.parent;
			int siblingIx = tileBack.GetSiblingIndex();
			int tileId = _tileTracker.GetLocContents(CLoc.LocalPrivateRack)[siblingIx];
			Destroy(tileBack.gameObject);

			Transform tile = _uiHandler.allTileTransforms[tileId];
			tile.SetParent(rack);
			tile.SetSiblingIndex(siblingIx);
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rack);
		}
	}
}