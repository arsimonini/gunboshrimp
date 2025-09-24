using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public GameState GameState;

    public int enemyCount;
    public int actionPoints = 0;

    public Dictionary<Vector2Int, TileType> specialTiles; 


    void Awake() {
        Instance = this;
    }

    void Start() {
        
        ChangeState(GameState.GenerateGrid);
    }

    void Update() {
    }

    public void ChangeState(GameState newState) {
        GameState = newState;

        switch(newState) {
            case GameState.GenerateGrid:
                Debug.Log("Changing State to Generate Grid");

                Vector2Int center = new Vector2Int(6,6);

                specialTiles = new Dictionary<Vector2Int, TileType> {
                    {center, TileType.Wall}, 
                    {center + Vector2Int.up, TileType.Wall},
                    {center + Vector2Int.down, TileType.Wall},
                    {center + Vector2Int.left, TileType.Wall},
                    {center + Vector2Int.right, TileType.Wall}
                };

                Grid.Instance.GenerateGrid(specialTiles);
                break;
            case GameState.SpawnHero:
                Debug.Log("Changing State to Spawn Hero");
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                Debug.Log("Changing State to Spawn Enemies");
                UnitManager.Instance.SpawnEnemies(enemyCount);
                break;
            case GameState.HeroTurn:
                Debug.Log("Changing State to Hero Turn");
                UnitManager.Instance.CheckActionPoints(UnitManager.Instance.heroReference);
                break;
            case GameState.EnemyTurn:
                Debug.Log("Changing State to Enemy Turn");
                StartCoroutine(EnemyTurnRoutine());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }
    }


    private IEnumerator EnemyTurnRoutine() {
        Debug.Log("Starting Enemy Turn");

        yield return UnitManager.Instance.ExecuteEnemyTurn();

        Debug.Log("Enemy Turn Complete. Returning to Hero Turn.");

        ChangeState(GameState.HeroTurn);
    }


}



public enum GameState
{
    GenerateGrid = 0,
    SpawnHero = 1,
    SpawnEnemies = 2,
    HeroTurn = 3,
    EnemyTurn = 4,
}
