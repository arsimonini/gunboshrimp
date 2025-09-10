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
    [SerializeField] private Tile normalTilePrefab;
    [SerializeField] private Tile wallTilePrefab;
    [SerializeField] private Tile lavaTilePrefab;
    [SerializeField] private Tile coralTilePrefab;
    [SerializeField] private Tile seaGrassTilePrefab;
    [SerializeField] private Tile doorTilePrefab;

    private Dictionary<TileType, Tile> tilePrefabs;
    
    //reference to main camera to center the view
    [SerializeField] private Transform cam;

    //Dictionary storing all tiles, using their grid position as the key
    [SerializeField] public Dictionary<Vector2Int, Tile> tiles;

    void Awake() {
        Instance = this;

        tilePrefabs = new Dictionary<TileType, Tile> {
            {TileType.Normal, normalTilePrefab}, 
            {TileType.Wall, wallTilePrefab}, 
            {TileType.Lava, lavaTilePrefab}, 
            {TileType.Coral, coralTilePrefab}, 
            {TileType.SeaGrass, seaGrassTilePrefab},
            {TileType.Door, doorTilePrefab}, 
        };
    }


    //Called to generate the grid layour based on the background image and grid size
    public void GenerateGrid(Dictionary<Vector2Int, TileType> specialTiles = null) {
        tiles = new Dictionary<Vector2Int, Tile>();

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
                Vector2Int gridPos = new Vector2Int(x, y);

                TileType type = TileType.Normal;


                //Edge Walls
                if(x == 0 || y == 0 || x == width - 1 || y == height - 1) {
                    type = TileType.Wall;
                }

                //Override with speical tile if defined
                if(specialTiles != null && specialTiles.TryGetValue(gridPos, out TileType customType)) {
                    type = customType;
                }

                //Get the correct prefab for the tile type, if doesn't work, goes to default
                if(!tilePrefabs.TryGetValue(type, out Tile prefabToUse)) {
                    Debug.LogWarning($"No prefab found for tile type {type}, defaulting to normal tile prefab.");
                    prefabToUse = normalTilePrefab;
                }

                //Instantiate the tile prefab at the calculated position
                var spawnedTile = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                //Set the tile's visual scale to fit the grid cell
                spawnedTile.transform.localScale = new Vector3(cellWidth, cellHeight, 1f);

                //Apply the tile type
                spawnedTile.setTileType(type);

                //Store the tile in the dictionary with its grid coordinates
                tiles[gridPos] = spawnedTile;
                
            }
        }

        //Move the camera to the center of the grid
        cam.transform.position = new Vector3(bounds.center.x, bounds.center.y - 7.5f, -5f);

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
    public Tile GetTileAtPosition(Vector2Int pos) {
        if(tiles.TryGetValue(pos, out var tile)) {
            return tile;
        }

        return null;
    }

}
