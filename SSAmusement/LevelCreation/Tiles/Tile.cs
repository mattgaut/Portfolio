using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Square, UpwardTriangle, DownwardTriangle, Platform }

public class Tile : MonoBehaviour {
    [SerializeField] TileType type;
    [SerializeField] SpriteRenderer top_border, left_border, right_border, bottom_border;
    SpriteRenderer sr;
    public Vector2Int position;

    public TileType tile_type {
        get { return type; }
    }

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void LoadSprite(TileSet s) {
        if (type == TileType.Square) {
            GetComponent<SpriteRenderer>().sprite = s.GetBaseSprite(this);
            bottom_border.sprite = s.GetBorderSprite(this);
            left_border.sprite = s.GetBorderSprite(this);
            right_border.sprite = s.GetBorderSprite(this);
            top_border.sprite = s.GetBorderSprite(this);
        }
        if (type == TileType.UpwardTriangle) {
            sr.sprite = s.GetBaseSprite(this);
            bottom_border.sprite = s.GetBorderSprite(this);
            left_border.sprite = s.GetDiagonalBorderSprite(this);
            right_border.sprite = s.GetBorderSprite(this);
            top_border.sprite = s.GetDiagonalBorderSprite(this);
        }
        if (type == TileType.DownwardTriangle) {
            sr.sprite = s.GetBaseSprite(this);
            bottom_border.sprite = s.GetBorderSprite(this);
            left_border.sprite = s.GetBorderSprite(this);
            right_border.sprite = s.GetDiagonalBorderSprite(this);
            top_border.sprite = s.GetDiagonalBorderSprite(this);
        }
        if (type == TileType.Platform) {
            GetComponent<SpriteRenderer>().sprite = s.GetBorderSprite(this);
        }
    }
    public void SetTopBorder(bool set) {
        if (type != TileType.Platform) top_border.enabled = (set);
    }
    public void SetBottomBorder(bool set) {
        if (type != TileType.Platform) bottom_border.enabled = (set);
    }
    public void SetLeftBorder(bool set) {
        if (type != TileType.Platform) left_border.enabled = (set);
    }
    public void SetRightBorder(bool set) {
        if (type != TileType.Platform) right_border.enabled = (set);
    }
}
