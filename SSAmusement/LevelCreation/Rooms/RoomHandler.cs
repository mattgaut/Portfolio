using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Room))]
public class RoomHandler : MonoBehaviour {

    Room room;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            if (collision.gameObject.transform.parent.position.y >= (room.transform.position.y + room.bottom_left.y + 1))RoomManager.instance.SetActiveRoom(room);
        }
    }

    private void Awake() {
        room = GetComponent<Room>();
        room.template.Ini();
        foreach (Tile t in GetComponentsInChildren<Tile>()) {
            if (t.tile_type != TileType.Platform) {
                if (t.tile_type == TileType.Square) {
                    t.GetComponent<BoxCollider2D>().usedByComposite = true;
                } else
                    t.GetComponent<PolygonCollider2D>().usedByComposite = true;
            }
        }
    }


}
