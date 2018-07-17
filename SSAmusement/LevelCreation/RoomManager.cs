using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    public static RoomManager instance {
        get; private set;
    }

    public Room active { get; private set; }

    [SerializeField] List<GameObject> objects_to_center_on_active_room;
    Dictionary<Room, List<Room>> rooms;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public void SetRooms(Dictionary<Room, List<Room>> rooms) {
        this.rooms = rooms;
    }

    public void SetActiveRoom(Room r) {
        if (r != active) {
            if (active) {
                active.OnDeactivate();
            }
            active = r;
            active.OnActivate();
            UIHandler.FocusRoom(active);
            if (active != null) {
                foreach (GameObject go in objects_to_center_on_active_room) {
                    go.transform.position = (Vector3)(active.top_right + active.bottom_left)/2 + active.transform.position;
                }
            }
        }
    }
}
