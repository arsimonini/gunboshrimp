using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Grid : MonoBehaviour
{
    //Singleton to allow global access
    public static Grid Instance;

    //Background image (determines grid bounds)
    [SerializeField] private SpriteRenderer backgroundImage;
    
    //Grid Dimensions (number of tiles)
    [SerializeField] private int width;
    [SerializeField] private int height;
    
    //Prefab to instantiate for each tile
    [SerializeField] private Tile tilePrefab;
    
    //reference to main camera to center the view
    [SerializeField] private Transform cam;

    //Dictionary storing all tiles, using their grid position as the key
    [SerializeField] private Dictionary<Vector2, Tile> tiles;

    void Awake() {
        Instance = this;
    }


    //Called to generate the grid layour based on the background image and grid size
    public void GenerateGrid() {
        tiles = new Dictionary<Vector2, Tile>();

        //Get the size of the background image in world units
        Bounds bounds = backgroundImage.bounds;
        float imageWidth = bounds.size.x;
        float imageHeight = bounds.size.y;

        //Calculate the size of each tile based on grid dimensions
        float cellWidth = imageWidth / width;
        float cellHeight = imageHeight / height;

        //bottom-left position of the grid (starting point)
        Vector2 bottomLeft = new Vector2(bounds.min.x, bounds.min.y);


        //For each grid cell create a tile at the correct position
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                //Calculate the position where teh tile should be spawned (center of cell)
                Vector2 spawnPos = bottomLeft + new Vector2(x * cellWidth + cellWidth / 2, y * cellHeight + cellHeight / 2);

                //Instantiate the tile prefav at the calculated position
                var spawnedTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                //Set the tile's visual scale to fit the grid cell
                spawnedTile.transform.localScale = new Vector3(cellWidth, cellHeight, 1f);
                
                //Store the tile in the dictionary with its grid coordinates
                tiles[new Vector2(x, y)] = spawnedTile;
                
            }
        }

        //Move the camera to the center of the grid
        cam.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10f);

        //Chate the state to spawn the heros
        GameManager.Instance.ChangeState(GameState.SpawnHero);
    }

    //Get a random walkable tile on the left half of hte grid for hero spawning
    public Tile GetHeroSpawnTile() {
        return tiles.Where(t => t.Key.x < width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }


    //Get a random walkable tile on the right half of the grid for enemy spawning
    public Tile GetEnemySpawnTile() {
        return tiles.Where(t => t.Key.x > width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    //Get the tile at a specific grid position, or return null if not found
    public Tile GetTileAtPosition(Vector2 pos) {
        if(tiles.TryGetValue(pos, out var tile)) {
            return tile;
        }

        return null;
    }

}
