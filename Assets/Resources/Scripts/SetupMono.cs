using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class SetupMono : MonoBehaviour
    {
        private Mono _mono;

        public void StartGame()
        {
            _mono = gameObject.AddComponent<Mono>();
            
            // generate tiles and add to tracker
            List<Tile> tiles = new TileGenerator().GenerateTiles();
            TileTrackerClient tileTracker = new(_mono, tiles);
            
            // make the game objects
            GenerateTileGameObjects(tileTracker);
            // TODO: next line will be conditional on this client being the host
            new SetupServer().StartGame(tileTracker.AllTiles);
            // RPC_C2S_RequestRack();
            
            // when done with setup, destroy this component
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
                Transform newTileMono = Instantiate(tilePrefab, pool).transform;
                // set reference on the Tile object
                tile.TileTransform = newTileMono;
                // set the game object name
                newTileMono.name = tile.ToString();
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
                        4 => "Plump",
                        5 => "Spring",
                        6 => "Summer",
                        _ => "Winter",
                    };
                }
                else imageName = newTileMono.name;
                newTileMono.GetComponentInChildren<Image>().sprite 
                    = UnityEngine.Resources.Load<Sprite>($"TileImages/{imageName}");
                
                _mono.AllTileTransforms.Add(newTileMono);
            }
        }
    }
}
