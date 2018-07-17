using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replace : MonoBehaviour {

    [SerializeField] Tile square, platform, ramp_up, ramp_down;
    [SerializeField] TileSet ts;

	// Use this for initialization
	void Start () {
		foreach (Tile t in GetComponentsInChildren<Tile>()) {
            if (t.tile_type == TileType.Square) {
                Tile new_tile = Instantiate(square, t.transform.position, Quaternion.identity);
                new_tile.position = t.position;
                new_tile.transform.SetParent(t.transform.parent);
                new_tile.LoadSprite(ts);
            }
            if (t.tile_type == TileType.Platform) {
                Tile new_tile = Instantiate(platform, t.transform.position, Quaternion.identity);
                new_tile.position = t.position;
                new_tile.transform.SetParent(t.transform.parent);
                new_tile.LoadSprite(ts);
            }
            if (t.tile_type == TileType.UpwardTriangle) {
                Tile new_tile = Instantiate(ramp_up, t.transform.position, Quaternion.identity);
                new_tile.position = t.position;
                new_tile.transform.SetParent(t.transform.parent);
                new_tile.LoadSprite(ts);
            }
            if (t.tile_type == TileType.DownwardTriangle) {
                Tile new_tile = Instantiate(ramp_down, t.transform.position, Quaternion.identity);
                new_tile.position = t.position;
                new_tile.transform.SetParent(t.transform.parent);
                new_tile.LoadSprite(ts);
            }
            Destroy(t.gameObject);
        }
	}
}
