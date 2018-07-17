using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryBox : MonoBehaviour {
    [SerializeField] EdgeCollider2D left, right, bottom, top;

    public void Set(Room room) {
        Vector2 offset = (Vector2.one * 0.5f);
        right.points = new Vector2[] { room.bottom_right, room.top_right };
        left.points = new Vector2[] { room.bottom_left, room.top_left };
        top.points = new Vector2[] { room.top_right, room.top_left };
        bottom.points = new Vector2[] {room.bottom_left, room.bottom_right };
    }

    public void Disable() {
        left.enabled = right.enabled = top.enabled = bottom.enabled = false;
    }

    public void Enable() {
        left.enabled = true;
        right.enabled = true;
        top.enabled = true;
        bottom.enabled = true;
    }
}
