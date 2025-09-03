using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public GameState GameState;

    public int enemyCount;


    void Awake() {
        Instance = this;
    }

    void Start() {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState) {
        GameState = newState;

        switch(newState) {
            case GameState.GenerateGrid:
                Grid.Instance.GenerateGrid();
                break;
            case GameState.SpawnHero:
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies(enemyCount);
                break;
            case GameState.HeroTurn:
                break;
            case GameState.EnemyTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }
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
