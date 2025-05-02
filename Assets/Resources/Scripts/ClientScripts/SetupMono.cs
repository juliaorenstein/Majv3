using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class SetupMono : MonoBehaviour
    {
        private UIHandlerMono _uiHandlerMono;
        private InputSender _inputSender;
        private TileTrackerClient _tileTracker;
        private NetworkedGameState _myGameState;
        private FusionManagerGlobal _fusionManager;

        public void SetUp(NetworkedGameState myNetworkedGameState)
        {
            Debug.Log("SetupMono.SetUp()");
            _uiHandlerMono = GetComponent<UIHandlerMono>();
            
            _inputSender = new();
            myNetworkedGameState.Runner.GetComponent<FusionEventHandler>().InputSender = _inputSender;
            
            // get my networked game state and exchange tile tracker client
            var fusionManagerGlobals = FindObjectsByType<FusionManagerGlobal>(FindObjectsSortMode.None);
            if (fusionManagerGlobals.Length > 1) throw new UnityException("There is more than one FusionManagerGlobal object.");
            _fusionManager = fusionManagerGlobals[0];
            
            // generate tiles and add to tracker
            List<Tile> tiles = new TileGenerator().GenerateTiles();
            _tileTracker = new(_uiHandlerMono, tiles, _inputSender, _fusionManager);
            
           myNetworkedGameState.TileTracker = _tileTracker;
           _tileTracker.GameState = myNetworkedGameState;
           _myGameState = myNetworkedGameState;
            
            // make the game objects
            GenerateTileGameObjects();
            PopulateOtherPrivateRacks();
            
            // find the button handler and add input there
            ButtonHandlerMono buttonHandler = GameObject.Find("Actions").GetComponent<ButtonHandlerMono>();
            buttonHandler.InputSender = _inputSender;
            
            // set up charleston stuff
            GameObject.Find("Charleston");
            
            Destroy(this);
        }

        private void GenerateTileGameObjects()
        {
            int flowerCounter = 0;
            GameObject tilePrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Tile");
            Transform pool = GameObject.Find("Pool").GetComponent<Transform>();
            
            foreach (Tile tile in _tileTracker.AllTiles)
            {
                // create tile game object
                Transform newTileTransform = Instantiate(tilePrefab, pool).transform;
                // set reference on the Tile object
                tile.TileTransform = newTileTransform;
                // set the game object name
                newTileTransform.name = tile.ToString();
                // set the tile image
                string imageName;
                if (tile.Wind == Wind.Flower)
                {
                    imageName = flowerCounter++ switch
                    {
                        0 => "Autumn",
                        1 => "Bamboo",
                        2 => "Chrys",
                        3 => "Orchid",
                        4 => "Plumb",
                        5 => "Spring",
                        6 => "Summer",
                        _ => "Winter",
                    };
                }
                else imageName = newTileTransform.name;
                newTileTransform.GetComponentInChildren<Image>().sprite 
                    = UnityEngine.Resources.Load<Sprite>($"TileImages/{imageName}");
                // set mono and _tileTracker on Drag Handler
                DragHandlerMono dragHandler = newTileTransform.GetComponentInChildren<DragHandlerMono>();
                dragHandler.uiHandlerMono = _uiHandlerMono;
                dragHandler.TileTracker = _tileTracker;
                dragHandler.tileId = tile.Id;
                dragHandler.InputSender = _inputSender;
                
                // add this to the list of tile transforms
                _uiHandlerMono.AllTileTransforms.Add(newTileTransform);
            }
        }

        private void PopulateOtherPrivateRacks()
        {
            GameObject tileBackPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Tile Back");
            int dealerIx = (_fusionManager.TurnPlayerIx - _fusionManager.LocalPlayerIx + 4) % 4;
            List<Transform> racks = new()
            {
                _uiHandlerMono.LocToTransform[CLoc.OtherPrivateRack1],
                _uiHandlerMono.LocToTransform[CLoc.OtherPrivateRack2],
                _uiHandlerMono.LocToTransform[CLoc.OtherPrivateRack3]
            };
            for (int rackIx = 1; rackIx <= 3; rackIx++)
            {
                Transform rack = racks[rackIx - 1];
                for (int _ = 0; _ < 13; _++)
                {
                    Instantiate(tileBackPrefab, rack);
                }
                // deal one extra to the dealer
                if (rackIx == dealerIx) Instantiate(tileBackPrefab, rack);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rack);
            }
        }
    }
}
