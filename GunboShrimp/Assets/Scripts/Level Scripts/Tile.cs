using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer ren;
    [SerializeField] private GameObject highlight;
    [SerializeField] private bool isWalkable;
    
    public BaseUnit OccupiedUnit;
    public bool Walkable => isWalkable && OccupiedUnit == null;

    //When the mouse enters the tile's area, show the highlight
    void OnMouseEnter() {
        highlight.SetActive(true);
    }

    //When the mouse leaves the tile's area, hide the highlight
    void OnMouseExit() {
        highlight.SetActive(false);
    }

    //When tile is clicked...
    void OnMouseDown() {
        // only respond to clicks on the hero's turn
        if(GameManager.Instance.GameState != GameState.HeroTurn) {
            return;
        }

        //if the tile has a unit on it...
        if(OccupiedUnit != null) {
            //...and it's a hero...
            if(OccupiedUnit.Faction == Faction.Hero) {
                //...select the hero
                UnitManager.Instance.setSelectedHero((BaseHero) OccupiedUnit);
            }
            //...otherwise, if it's an enemy
            else {
                //...and the hero is selected
                if(UnitManager.Instance.selectedHero != null) {
                    //...deselect the hero
                    UnitManager.Instance.setSelectedHero(null);
                }
            }
        }
    }

    //Assigns a unit to this tile, updating references both ways
    public void SetUnit(BaseUnit unit) {

        //if the unit was on another tile, clear that tile's reference
        if(unit.OccupiedTile != null) {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        
        //Move the unit's position to this tile's position
        unit.transform.position = transform.position;

        //Set up two-way references between unit and tile
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public void setTileType(TileType type) {
        switch(type) {
            case TileType.Wall:
                isWalkable = false;
                ren.color = Color.red;
                break;
            case TileType.Lava:
                isWalkable = true;
                //ren.color = Color.red;
                break;
            case TileType.Coral:
                isWalkable = false;
                ren.color = Color.pink;
                break;
            case TileType.SeaGrass:
                isWalkable = false;
                ren.color = Color.green;
                break;
            case TileType.Door:
                isWalkable = true;
                ren.color = Color.white;
                break;
            case TileType.Normal:
                isWalkable = true;
                ren.color = Color.white;
                break;
            default:
                isWalkable = true;
                ren.color = Color.white;
                break;
        }
    }

}


    public enum TileType {
        Normal = 0,
        Wall = 1,
        Lava = 2,
        Coral = 3,
        SeaGrass = 4,
        Door = 5,
    }

