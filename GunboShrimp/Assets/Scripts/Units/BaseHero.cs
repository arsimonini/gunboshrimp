using UnityEngine;
using System;

public class BaseHero : BaseUnit
{
    public int maxActionPoints;
    public int currentActionPoints;


    protected virtual void Start() {
        currentActionPoints = maxActionPoints;
    }

    protected virtual void Update()
    {
        //If hero is currently moving, the gamestate is not HeroTurn, or action points are less than or equal to 0, ignore input
        if (currentlyMoving || GameManager.Instance.GameState != GameState.HeroTurn || currentActionPoints <= 0) {
            return;
        }

        HandleMovementInput();
    }

    // Virtual method to override in subclasses for unique movement input/options
    protected virtual void HandleMovementInput()
    {
        // Default movement: WASD, cost 1 AP each
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
    }

    protected void TryMove(MoveDirection direction, int distance, int apCost)
    {
        if (currentActionPoints < apCost) {
            return; // Not enough AP
        }

        // Start movement coroutine with a callback to deduct AP after movement finishes
        moveUnit(direction, distance, () =>
        {
            currentActionPoints -= apCost;
            GameManager.Instance.actionPoints = currentActionPoints; // Sync global AP if used

            Debug.Log($"{name} moved {direction}. AP left: {currentActionPoints}");

            //End turn if out of AP
            if(currentActionPoints <= 0) {
                GameManager.Instance.ChangeState(GameState.EnemyTurn);
            }
        });
    }

    public void ResetActionPoints() {
        currentActionPoints = maxActionPoints;
        GameManager.Instance.actionPoints = maxActionPoints;
    }
}
