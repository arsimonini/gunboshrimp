using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    //List of all available scriptable units
    private List<ScriptableUnit> units;

    //The hero currently selected by the player
    public BaseHero selectedHero;

    void Awake() {
        Instance = this;

        //Load all Scriptable Units directly from the Resources folder (root)
        units = Resources.LoadAll<ScriptableUnit>("").ToList();
    }

    //Spawns a set number of heroes onto random valid tiles
    public void SpawnHeroes() {
        var heroCount = 1;

        
        for(int i = 0; i < heroCount; i++) {
            var randomPrefab = getRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);

            var randomSpawnTile = Grid.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);

        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies(int enCount) {
        var enemyCount = enCount;

        // if(enemyCount == 0) {
        //     GameManager.Instance.ChangeState(GameState.HeroTurn);
        // }

        for(int i = 0; i < enemyCount; i++) {
            var randomPrefab = getRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);

            var randomSpawnTile = Grid.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);

        }

        GameManager.Instance.ChangeState(GameState.HeroTurn);
    }

    private T getRandomUnit<T> (Faction faction) where T : BaseUnit {
        return (T) units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().unitPrefab;
    }

    public void setSelectedHero(BaseHero hero) {
        selectedHero = hero;
    }
}
