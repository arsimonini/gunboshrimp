using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BaseUnit : MonoBehaviour
{

    public Tile OccupiedTile;
    public Faction Faction;
    public bool currentlyMoving = false;

    
    //Public method to move the unit in a specified direction and distance
    public void moveUnit(MoveDirection direction, int distance, System.Action onComplete = null) {
        
        if(currentlyMoving) {
            return;
        }

        currentlyMoving = true;

        //Coroutines handle movement over time
        StartCoroutine(MoveRoutine(direction, distance, onComplete));
        
    }


    // Coroutine that handles step by step movement across tiles
    public IEnumerator MoveRoutine(MoveDirection direction, int distance, System.Action onComplete = null) {
        
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
                yield break;
            }

            //Smoothly move the unit to the next tile
            yield return StartCoroutine(MoveToTile(nextTile));

            //Update current position and occupied tile
            currentPos = nextPos;
            OccupiedTile = nextTile;
        }

        currentlyMoving = false;

        //Call the callback to notify that movement finished
        onComplete?.Invoke();
    }


    private IEnumerator MoveToTile(Tile targetTile) {
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


}
