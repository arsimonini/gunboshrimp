using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnPiranha : BaseEnemy
{
    public override IEnumerator takeTurn()
    {
        Debug.Log($"{name}: EnPiranha.takeTurn() started.");

        while (currentActionPoints > 0)
        {
            Debug.Log($"{name}: AP = {currentActionPoints}");

            BaseHero closestHero = findClosestHero();
            if (closestHero == null)
            {
                Debug.Log($"{name}: No hero found");
                yield break;
            }

            var path = findPath(this.OccupiedTile, closestHero.OccupiedTile);
            if (path == null || path.Count < 2)
            {
                Debug.Log($"{name}: No path to hero");
                yield break;
            }

            Tile nextTile = path[1];
            Debug.Log($"{name}: Moving to {nextTile.gridPosition}");

            yield return StartCoroutine(MoveToTile(nextTile));

            currentActionPoints--;

            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log($"{name}: Finished turn.");
    }


    private BaseHero findClosestHero()
    {
        BaseHero closest = null;
        float closestDist = float.MaxValue;

        foreach (var hero in UnitManager.Instance.heroes)
        {
            float dist = Vector2Int.Distance(this.OccupiedTile.gridPosition, hero.OccupiedTile.gridPosition);
            if(dist < closestDist)
            {
                closestDist = dist;
                closest = hero;
            }
        }

        return closest;
    }
}
