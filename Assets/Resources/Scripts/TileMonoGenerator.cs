using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class TileMonoGenerator : MonoBehaviour
    {
        private GameObject _tilePrefab;
        private Transform _pool;

        private void Start()
        {
            _tilePrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Tile");
            _pool = GameObject.Find("Pool").GetComponent<Transform>();
            GenerateTiles();
        }

        void GenerateTiles()
        {
            TileGenerator tileGenerator = new();
            tileGenerator.GenerateTiles();
            int flowerCounter = 0;
            
            foreach (Tile tile in Tile.AllTiles)
            {
                // create tile game object
                Transform newTileMono = Instantiate(_tilePrefab, _pool).transform;
                // set reference on the Tile object
                tile.transform = newTileMono;
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
            }
        }
    }
}
