using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

public class BaseUnit : MonoBehaviour
{

    public Tile OccupiedTile;
    public Faction Faction;
    public bool currentlyMoving = false;

    
    //Public method to move the unit in a specified direction and distance
    public void moveUnit(MoveDirection direction, int distance) {
        
        currentlyMoving = true;

        //Coroutines handle movement over time
        StartCoroutine(MoveRoutine(direction, distance));
        
    }


    // Coroutine that handles step by step movement across tiles
    public IEnumerator MoveRoutine(MoveDirection direction, int distance) {
        
        //Gets current position of the unit on the grid
        Vector2Int currentPos = Grid.Instance.tiles.FirstOrDefault(t => t.Value == OccupiedTile).Key;
        
        //Covert the direction enum into a directional vector
        Vector2Int dirVector = DirectionToVector2Int(direction);

        //Move step by step in a specified direction for the given distance
        for (int i = 0; i < distance; i++) {
            Vector2Int nextPos = currentPos + dirVector;
            
            //gets the tiles next position
            Tile nextTile = Grid.Instance.GetTileAtPosition(nextPos);

            //Stops the movement if the tile is invalid or not walkable
            if(nextTile == null || !nextTile.Walkable) {
                Debug.Log("Cannot move further, tile is invalid or not walkable");
                currentlyMoving = false;
                break;
            }

            //Smoothly move the unit to the next tile
            yield return StartCoroutine(MoveToTile(nextTile));

            //Update current position and occupied tile
            currentPos = nextPos;
            OccupiedTile = nextTile;
        }
    }


    protected IEnumerator MoveToTile(Tile targetTile) {
        Vector3 startPos = transform.position;
        Vector3 endPos = targetTile.transform.position;

        float elapsed = 0f;
        float duration = 0.3f;

        //Interpolate the unit's position over time
        while (elapsed < duration) {
            transform.position = Vector3.Lerp(startPos, endPos, (elapsed/duration));
            elapsed += Time.deltaTime;
            yield return null; // wait until next frame
        }

        //Snap to final position and assing the unit to the new tile
        transform.position = endPos;
        targetTile.SetUnit(this);
        currentlyMoving = false;
    }

    private Vector2Int DirectionToVector2Int(MoveDirection direction) {
        switch(direction) {
            case MoveDirection.Up:
                return new Vector2Int(0, 1);
            case MoveDirection.Down:
                return new Vector2Int(0, -1);
            case MoveDirection.Left:
                return new Vector2Int(-1, 0);
            case MoveDirection.Right:
                return new Vector2Int(1, 0);
            case MoveDirection.NorthEast:
                return new Vector2Int(1, 1);
            case MoveDirection.NorthWest:
                return new Vector2Int(-1, 1);
            case MoveDirection.SouthEast:
                return new Vector2Int(1, -1);
            case MoveDirection.SouthWest:
                return new Vector2Int(-1, -1);
            
            default:
                return Vector2Int.zero;

        }
    }




    public enum MoveDirection {
        Up,
        Down,
        Left,
        Right,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
    }

    protected List<Tile> findPath(Tile startTile, Tile goalTile)
    {
        Vector2Int start = startTile.gridPosition;
        Vector2Int goal = goalTile.gridPosition;

        // Maps each node to its immediate predecessor
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Cost from start to node
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };

        // Estimated total cost (start to goal through node)
        var fScore = new Dictionary<Vector2Int, int> { [start] = Heuristic(start, goal) };


        // Heuristic function
        int Heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan
        }

        // Set of discovered nodes to be evaluated
        var openSet = new SortedSet<Vector2Int>(Comparer<Vector2Int>.Create((a, b) =>
        {
            int compare = fScore.GetValueOrDefault(a, int.MaxValue).CompareTo(fScore.GetValueOrDefault(b, int.MaxValue));
            return compare == 0 ? a.GetHashCode().CompareTo(b.GetHashCode()) : compare;
        }));
        openSet.Add(start);

  
        while (openSet.Count > 0)
        {
            // Node in openSet with the lowest fScore
            Vector2Int current = openSet.Min;
            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (Vector2Int direction in Directions)
            {
                Vector2Int neighbor = current + direction;
                Tile neighborTile = Grid.Instance.GetTileAtPosition(neighbor);

                if (neighborTile == null || !neighborTile.Walkable)
                    continue;

                int tentativeGScore = gScore.GetValueOrDefault(current, int.MaxValue) + 1;

                if (tentativeGScore < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // Failed to find a path
        return null;
    }

    // Reconstructs the path from cameFrom map
    private List<Tile> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Tile> totalPath = new List<Tile>();
        totalPath.Add(Grid.Instance.GetTileAtPosition(current));

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(Grid.Instance.GetTileAtPosition(current));
        }

        totalPath.Reverse();
        return totalPath;
    }

    private readonly List<Vector2Int> Directions = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };


}
