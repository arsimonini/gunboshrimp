using UnityEngine;

public class GunboShrimp : BaseHero
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        maxActionPoints = 3;
        base.Start();
    }


    protected override void HandleMovementInput()
    {
        // Same controls you had before
        if (Input.GetKeyDown(KeyCode.W)) {
            TryMove(MoveDirection.Up, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            TryMove(MoveDirection.Down, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.A)) {
            TryMove(MoveDirection.Left, 1, 1);
        }
            
        else if (Input.GetKeyDown(KeyCode.D)) {
            TryMove(MoveDirection.Right, 1, 1);
        }

        else if (Input.GetKeyDown(KeyCode.Q)) {
            TryMove(MoveDirection.NorthWest, 1, 1);
        }
            
        else if (Input.GetKeyDown(KeyCode.E)) {
            TryMove(MoveDirection.NorthEast, 1, 1);
        }
            
        else if (Input.GetKeyDown(KeyCode.C)) {
            TryMove(MoveDirection.SouthEast, 1, 1);
        }
            
        else if (Input.GetKeyDown(KeyCode.Z)) {
            TryMove(MoveDirection.SouthWest, 1, 1);
        }
    }
}
