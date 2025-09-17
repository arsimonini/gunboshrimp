using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    public string name;
    public int APcost;
    public string type;
    public bool targeted;

    public bool hovered = false;

    public bool holding = false;

    [Header("Targeting Settings")]
    public string targetingType; //Ex. Square, Diamond, Other -> Links to custom targeting
    public int amountOfTargets;
    public bool needDifferentTargets;
    public bool targetTiles;
}
