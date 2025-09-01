using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private int width;
    [SerializeField] private int height;
    //private float cellSize;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;

    [SerializeField] private Dictionary<Vector2, Tile> tiles;

    void Start() {
        GenerateGrid();
    }

    void GenerateGrid() {
        tiles = new Dictionary<Vector2, Tile>();

        Bounds bounds = backgroundImage.bounds;
        float imageWidth = bounds.size.x;
        float imageHeight = bounds.size.y;

        float cellWidth = imageWidth / width;
        float cellHeight = imageHeight / height;

        Vector2 bottomLeft = new Vector2(bounds.min.x, bounds.min.y);

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {

                Vector2 spawnPos = bottomLeft + new Vector2(x * cellWidth + cellWidth / 2, y * cellHeight + cellHeight / 2);

                var spawnedTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.localScale = new Vector3(cellWidth, cellHeight, 1f);

                tiles[new Vector2(x, y)] = spawnedTile;
                
            }
        }

        cam.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10f);
    }

    public Tile GetTileAtPosition(Vector2 pos) {
        if(tiles.TryGetValue(pos, out var tile)) {
            return tile;
        }

        return null;
    }

}
