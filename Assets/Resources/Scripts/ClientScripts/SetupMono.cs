using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class SetupMono : MonoBehaviour
    {
        private Mono _mono;

        public TileTrackerClient StartGame()
        {
            GameObject gameManager = GameObject.Find("GameManager");
            _mono = gameManager.GetComponent<Mono>();
            
            // Set up fusion manager
            GameObject fusionManager = GameObject.Find("FusionManager");
            FusionManagerClient fusionManagerClient = fusionManager.GetComponent<FusionManagerClient>();
            
            // generate tiles and add to tracker
            List<Tile> tiles = new TileGenerator().GenerateTiles();
            TileTrackerClient tileTracker = new(_mono, tiles, fusionManagerClient);
            
            // make the game objects
            GenerateTileGameObjects(tileTracker);
                
            // when done with setup, destroy the button this component
            Destroy(this);
            return tileTracker;
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
                // add this to the list of tile transforms
                _mono.AllTileTransforms.Add(newTileTransform);
            }
        }
    }
}
