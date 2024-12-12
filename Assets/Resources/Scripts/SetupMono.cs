using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class SetupMono : MonoBehaviour
    {
        private Mono _mono;
        
        public void StartGame()
        {
            _mono = new();
            TileSetup();
        }

        void TileSetup()
        {
            TileTrackerClient tileTracker = new(_mono);
            new TileGenerator().GenerateTiles(tileTracker);
            GenerateTileGameObjects(tileTracker);
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
