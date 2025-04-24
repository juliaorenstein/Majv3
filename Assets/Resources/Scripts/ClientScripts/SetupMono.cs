using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class SetupMono : MonoBehaviour
    {
        private Mono _mono;
        private InputSender _inputSender;
        private TileTrackerClient _tileTracker;

        public void StartGame(NetworkedGameState myNetworkedGameState)
        {
            Debug.Log("SetupMono.StartGame()");
            _mono = GetComponent<Mono>();
            
            _inputSender = new();
            myNetworkedGameState.Runner.GetComponent<FusionEventHandler>().InputSender = _inputSender;
            
            // generate tiles and add to tracker
            List<Tile> tiles = new TileGenerator().GenerateTiles();
            _tileTracker = new(_mono, tiles, _inputSender);
            
            // get my networked game state and exchange tile tracker client
            var fusionManagerGlobals = FindObjectsByType<FusionManagerGlobal>(FindObjectsSortMode.None);
            if (fusionManagerGlobals.Length > 1) throw new UnityException("There is more than one FusionManagerGlobal object.");
            FusionManagerGlobal fusionManagerGlobal = fusionManagerGlobals[0];
            
           myNetworkedGameState.TileTracker = _tileTracker;
           _tileTracker.GameState = myNetworkedGameState;
            
            // make the game objects
            GenerateTileGameObjects(_tileTracker);
            
            // find the button handler and add input there
            ButtonHandlerMono buttonHandler = GameObject.Find("Actions").GetComponent<ButtonHandlerMono>();
            buttonHandler.InputSender = _inputSender;
            
            Destroy(this);
        }

        void GenerateTileGameObjects(TileTrackerClient tileTracker)
        {
            int flowerCounter = 0;
            GameObject tilePrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Tile");
            Transform pool = GameObject.Find("Pool").GetComponent<Transform>();
            
            foreach (Tile tile in tileTracker.AllTiles)
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
                // set mono and tileTracker on Drag Handler
                DragHandlerMono dragHandler = newTileTransform.GetComponentInChildren<DragHandlerMono>();
                dragHandler.mono = _mono;
                dragHandler.TileTracker = tileTracker;
                dragHandler.tileId = tile.Id;
                dragHandler.InputSender = _inputSender;
                // add this to the list of tile transforms
                _mono.AllTileTransforms.Add(newTileTransform);
            }
        }
    }
}
