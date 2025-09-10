using UnityEngine;

public class GunboShrimp : BaseHero
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentlyMoving == false  && GameManager.Instance.GameState == GameState.HeroTurn) {
            if(Input.GetKeyDown(KeyCode.S)) {
                moveUnit(MoveDirection.Down, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }

            if(Input.GetKeyDown(KeyCode.W)) {
                moveUnit(MoveDirection.Up, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }

            if(Input.GetKeyDown(KeyCode.A)) {
                moveUnit(MoveDirection.Left, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }

            if(Input.GetKeyDown(KeyCode.D)) {
                moveUnit(MoveDirection.Right, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }

            if(Input.GetKeyDown(KeyCode.Q)) {
                moveUnit(MoveDirection.NorthWest, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }

            if(Input.GetKeyDown(KeyCode.E)) {
                moveUnit(MoveDirection.NorthEast, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }

            if(Input.GetKeyDown(KeyCode.Z)) {
                moveUnit(MoveDirection.SouthWest, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }

            if(Input.GetKeyDown(KeyCode.C)) {
                moveUnit(MoveDirection.SouthEast, 1);
                GameManager.Instance.actionPoints = GameManager.Instance.actionPoints - 1;
            }
        }
        
    }
}
