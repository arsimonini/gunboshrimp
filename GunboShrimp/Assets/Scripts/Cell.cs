using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{
    public int index;
    public int value;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer roomSprite;

    public void setSpecialRoomSprite(Sprite icon) {
        spriteRenderer.sprite = icon;
    }

    public void setRoomSprite(Sprite roomIcon) {
        roomSprite.sprite = roomIcon;
    }

    public void rotateCell(List<int> connectedCells) {
        connectedCells.Sort();
        index = connectedCells[0];

        if(connectedCells.Contains(index + 1) && connectedCells.Contains(index + 10))
        {
            applyRotation(-90);
        }
        else if(connectedCells.Contains(index + 1) && connectedCells.Contains(index + 11))
        {
            applyRotation(180);
        }
        else if(connectedCells.Contains(index + 9) && connectedCells.Contains(index + 10))
        {
            applyRotation(90);
        }

    }

    public void applyRotation(float angle) {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
