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
        if(GameState == GameState.HeroTurn && actionPoints == 0) {
            ChangeState(GameState.EnemyTurn);
        }
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
                UnitManager.Instance.CheckActionPoints();
                break;
            case GameState.EnemyTurn:
                Debug.Log("Changing State to Enemy Turn");
                StartCoroutine(handleEnemyTurn());
                //handleEnemyTurn();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }
    }

    private IEnumerator handleEnemyTurn()
    {
        Debug.Log("handleEnemyTurn() started");

        foreach (var enemy in UnitManager.Instance.enemies)
        {
            enemy.resetActionPoints();
            Debug.Log($"Enemy {enemy.name} starting turn with {enemy.currentActionPoints} AP");

            // ?? Safety timeout: prevent infinite coroutine
            float timeout = 5f;
            bool done = false;

            StartCoroutine(EnemyTurnWithCallback(enemy, () => done = true));

            while (!done && timeout > 0f)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            if (!done)
            {
                Debug.LogWarning($"Enemy {enemy.name} turn timed out!");
            }
        }

        Debug.Log("All enemies finished their turn");

        ChangeState(GameState.HeroTurn);
    }

    private IEnumerator EnemyTurnWithCallback(BaseEnemy enemy, Action onComplete)
    {
        yield return StartCoroutine(enemy.takeTurn());
        onComplete?.Invoke();
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


