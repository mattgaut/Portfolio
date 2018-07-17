using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSet : MonoBehaviour {
    public string theme_name = "ClassicSpaceShip";
    public List<SpriteList> foreground;
    public List<Sprite> diagonal_borders, borders;

    public Sprite GetBaseSprite(Tile t) {
        int i, j;

        if (t.position.x >= 0) {
            i = t.position.x % foreground.Count;
        } else {
            i = (foreground.Count - 1) - (-(t.position.x + 1) % foreground.Count);
        }
        if (t.position.y >= 0) {
            j = t.position.y % foreground[i].list.Count;
        } else {
            j = (foreground[i].list.Count - 1) - (-(t.position.y + 1) % foreground[i].list.Count);
        }

        return foreground[i].list[j];
    }

    public Sprite GetBorderSprite(Tile t) {
        int i = (t.position.x + t.position.y);
        if (i >= 0) {
            i = i % borders.Count;
        } else {
            i = (borders.Count - 1) - (-(i + 1) % borders.Count);
        }
        return borders[i];
    }
    public Sprite GetDiagonalBorderSprite(Tile t) {
        int i = t.position.x + t.position.y;
        if (i >= 0) {
            i = i % diagonal_borders.Count;
        } else {
            i = (diagonal_borders.Count - 1) - (-(i + 1) % diagonal_borders.Count);
        }
        return diagonal_borders[i];
    }
}

[System.Serializable]
public class SpriteList {
    public List<Sprite> list;
}
