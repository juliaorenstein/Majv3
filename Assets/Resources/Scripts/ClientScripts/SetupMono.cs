using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class SetupMono : MonoBehaviour
    {
        private Mono _mono;
        private InputSender _inputSender;
        private TileTrackerClient _tileTracker;

        public void StartGame(int playerId, out InputSender inputSender)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            _mono = gameManager.GetComponent<Mono>();
            
            _inputSender = new();
            inputSender = _inputSender;
            
            // generate tiles and add to tracker
            List<Tile> tiles = new TileGenerator().GenerateTiles();
            _tileTracker = new(_mono, tiles, inputSender);
            
            // make the game objects
            GenerateTileGameObjects(_tileTracker);
            
            // find the button handler and add input there
            ButtonHandlerMono buttonHandler = GameObject.Find("Actions").GetComponent<ButtonHandlerMono>();
            buttonHandler.InputSender = _inputSender;
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
                // set mono and tiletracker on Drag Handler
                DragHandlerMono dragHandler = newTileTransform.GetComponentInChildren<DragHandlerMono>();
                dragHandler.mono = _mono;
                dragHandler.TileTracker = tileTracker;
                dragHandler.tileId = tile.Id;
                dragHandler.InputSender = _inputSender;
                // add this to the list of tile transforms
                _mono.AllTileTransforms.Add(newTileTransform);
            }
        }

        public void ConnectTileTrackerToNetworkedGameState(NetworkedGameState networkedGameState)
        {
            networkedGameState.TileTracker = _tileTracker;
            _tileTracker.GameState = networkedGameState;
            // update the game state on the client side to start the game!
            _tileTracker.UpdateGameState();
            // when done with setup, destroy the button this component
            Destroy(this);
        }
    }
}
