using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

namespace Resources
{
    public class SetupMono : MonoBehaviour
    {
        private Mono _mono;

        public void StartGame(RpcS2CHandler rpcS2CHandler)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            _mono = gameManager.AddComponent<Mono>();
            
            // generate tiles and add to tracker
            List<Tile> tiles = new TileGenerator().GenerateTiles();
            TileTrackerClient tileTracker = new(_mono, tiles);
            rpcS2CHandler.tileTracker = tileTracker;
            
            // make the game objects
            GenerateTileGameObjects(tileTracker);
            // TODO: next line will be conditional on this client being the host
            // BUG: RPC to server instead of calling directly
            // new SetupServer().StartGame(tileTracker.AllTiles);
            // RPC_C2S_RequestRack();
            // TODO: the following is for testing
            GameObject dealMe = GameObject.Find("Deal Me");
            dealMe.GetComponent<DealMeTest>().tileTracker = tileTracker;
                            
            // when done with setup, destroy the button this component
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
                        4 => "Plump",
                        5 => "Spring",
                        6 => "Summer",
                        _ => "Winter",
                    };
                }
                else imageName = newTileTransform.name;
                newTileTransform.GetComponentInChildren<Image>().sprite 
                    = UnityEngine.Resources.Load<Sprite>($"TileImages/{imageName}");
                // set mono and tiletracker on Drag Handler
                newTileTransform.GetComponent<DragHandlerMono>().mono = _mono;
                newTileTransform.GetComponent<DragHandlerMono>().TileTracker = tileTracker;
                newTileTransform.GetComponent<DragHandlerMono>().tileId = tile.Id;
                // add this to the list of tile transforms
                _mono.AllTileTransforms.Add(newTileTransform);
            }
        }
    }
}
