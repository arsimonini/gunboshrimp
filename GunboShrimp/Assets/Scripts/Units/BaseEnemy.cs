using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEnemy : BaseUnit
{

    public int maxActionPoints = 3;
    public int currentActionPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        currentActionPoints = maxActionPoints;
    }

    public void ResetActionPoints() {
        currentActionPoints = maxActionPoints;
    }

    public virtual IEnumerator MoveRandom() {
        //Randomly Pick a direction
        MoveDirection[] directions = (MoveDirection[]) System.Enum.GetValues(typeof(MoveDirection));

        //Shuffle direction to try multiple if blocked
        List<MoveDirection> shuffled = new List<MoveDirection>(directions);
        for(int i = 0; i < shuffled.Count; i++) {
            MoveDirection temp = shuffled[i];
            int randomIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        foreach (var dir in shuffled) {
            bool moved = false;
            // Start coroutine and wait for it to finish
            yield return StartCoroutine(moveUnitAndTrack(dir, 1, () => {
                moved = true;
                currentActionPoints--;
            }));


            if(moved) {
                yield return new WaitForSeconds(0.2f); // Pause between moves
                break;
            }
        }
        yield return null;
    }

    private IEnumerator moveUnitAndTrack(MoveDirection dir, int dist, System.Action onComplete) {
        bool isDone = false;

        moveUnit(dir, dist, () => {
            onComplete?.Invoke();
            isDone = true;
        });

        // Wait until movement is complete
        while (!isDone) {
            yield return null;
        }
    }

    
}
