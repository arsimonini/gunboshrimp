using System.Collections;    
using System.Collections.Generic;
using UnityEngine;


public class BaseEnemy : BaseUnit
{

    public int currentActionPoints = 0;
    public int maxActionPoints = 3;


    public virtual void resetActionPoints()
    {
        currentActionPoints = 3;
    }

    public virtual IEnumerator takeTurn()
    {
        Debug.Log($"{name}: BaseEnemy.takeTurn() called.");
        yield return null;
    }




}
