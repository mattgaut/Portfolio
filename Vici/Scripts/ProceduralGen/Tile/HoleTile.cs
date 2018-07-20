using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTile : Tile {

    [SerializeField]
    Sprite no_neighbors, one_neighbor, two_neighbors_straight, two_neighbors_corner, three_neighbors, four_neighbors;

    [SerializeField]
    Sprite filled_tr, empty_tr, filled_br, empty_br, filled_bl, empty_bl, filled_tl, empty_tl;

    [SerializeField]
    GameObject main, tr_corner, br_corner, bl_corner, tl_corner;

    protected override void SetUp() {
        SetSprites();
    }

    void SetSprites() {
        int count_neighbors = 0;

        bool has_right = true, has_left = true, has_top = true, has_bottom = true;

        IntTuple right_neighbor = new IntTuple(position.x + 1, position.y);
        IntTuple left_neighbor = new IntTuple(position.x - 1, position.y);
        IntTuple top_neighbor = new IntTuple(position.x, position.y + 1);
        IntTuple bottom_neighbor = new IntTuple(position.x, position.y - 1);

        if (room_layout.sorted_tiles.ContainsKey(top_neighbor) && room_layout.sorted_tiles[top_neighbor].GetType() == GetType()) {
            count_neighbors++;
        } else {
            has_top = false;
        }
        if (room_layout.sorted_tiles.ContainsKey(bottom_neighbor) && room_layout.sorted_tiles[bottom_neighbor].GetType() == GetType()) {
            count_neighbors++;
        } else {
            has_bottom = false;
        }
        if (room_layout.sorted_tiles.ContainsKey(left_neighbor) && room_layout.sorted_tiles[left_neighbor].GetType() == GetType()) {
            count_neighbors++;
        } else {
            has_left = false;
        }
        if (room_layout.sorted_tiles.ContainsKey(right_neighbor) && room_layout.sorted_tiles[right_neighbor].GetType() == GetType()) {
            count_neighbors++;
        } else {
            has_right = false;
        }

        int rotation = 0;
        if (count_neighbors == 4) {
            main.GetComponent<SpriteRenderer>().sprite = four_neighbors;
        } else if (count_neighbors == 3) {
            main.GetComponent<SpriteRenderer>().sprite = three_neighbors;
            if (!has_bottom) {
                rotation = 90;
            } else if (!has_top) {
                rotation = -90;
            } else if (!has_right) {
                rotation = 180;
            }
        } else if (count_neighbors == 2) {
            if ((has_bottom && has_top) || (has_right && has_left)) {
                main.GetComponent<SpriteRenderer>().sprite = two_neighbors_straight;
                if (!has_bottom) rotation = 90;
            } else {
                main.GetComponent<SpriteRenderer>().sprite = two_neighbors_corner;
                if (has_top && has_left) {
                    main.GetComponent<SpriteRenderer>().flipX = true;
                } else if (has_left && has_bottom) {
                    rotation = 180;
                } else if (has_bottom && has_right) {
                    main.GetComponent<SpriteRenderer>().flipY = true;
                }
            }
        } else if (count_neighbors == 1) {
            main.GetComponent<SpriteRenderer>().sprite = one_neighbor;
            if (has_bottom) {
                rotation = 180;
            } else if (has_left) {
                rotation = 90;
            } else if (has_right) {
                rotation = -90;
            }
        } else {
            main.GetComponent<SpriteRenderer>().sprite = no_neighbors;
        }
        main.transform.eulerAngles = new Vector3(0, 0, rotation);

        IntTuple tr = new IntTuple(position.x + 1, position.y + 1);
        IntTuple br = new IntTuple(position.x + 1, position.y - 1);
        IntTuple bl = new IntTuple(position.x - 1, position.y - 1);
        IntTuple tl = new IntTuple(position.x - 1, position.y + 1);

        if (has_top && has_right) {
            tr_corner.SetActive(true);
            tr_corner.GetComponent<SpriteRenderer>().sprite =
                (!room_layout.sorted_tiles.ContainsKey(tr) || room_layout.sorted_tiles[tr].GetType() != GetType()) ? filled_tr : empty_tr;
        }
        if (has_top && has_left) {
            tl_corner.SetActive(true);
            tl_corner.GetComponent<SpriteRenderer>().sprite =
                (!room_layout.sorted_tiles.ContainsKey(tl) || room_layout.sorted_tiles[tl].GetType() != GetType()) ? filled_tl : empty_tl;
        }
        if (has_bottom && has_left) {
            bl_corner.SetActive(true);
            bl_corner.GetComponent<SpriteRenderer>().sprite =
                (!room_layout.sorted_tiles.ContainsKey(bl) || room_layout.sorted_tiles[bl].GetType() != GetType()) ? filled_bl : empty_bl;
        }
        if (has_bottom && has_right) {
            br_corner.SetActive(true);
            br_corner.GetComponent<SpriteRenderer>().sprite =
                (!room_layout.sorted_tiles.ContainsKey(br) || room_layout.sorted_tiles[br].GetType() != GetType()) ? filled_br : empty_br;
        }
    }
}
