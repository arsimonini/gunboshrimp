using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnPiranha : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        maxActionPoints = 2;
        base.Start();
    }

    public override IEnumerator MoveRandom() {
        yield return base.MoveRandom(); // default random movement
    }

 
}
