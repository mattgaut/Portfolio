using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    protected RoomLayout room_layout;
    protected IntTuple position;
    [SerializeField]
    bool _walkable;
    public bool walkable {
        get { return _walkable; }
    }

    public void SetTile(RoomLayout rl, IntTuple pos) {
        room_layout = rl;
        position = pos;
        SetUp();
    }

    protected virtual void SetUp() {

    }
}
