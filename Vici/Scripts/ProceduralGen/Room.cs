using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public static readonly float room_width = 26, room_height = 14;

    [SerializeField]
    private GameObject door;
    [SerializeField]
    private GameObject long_wall, long_door_wall, short_wall, short_door_wall;
    [SerializeField]
    GameObject floor;

    public Room left_neighbor {
        get; private set;
    }
    public Room right_neighbor {
        get; private set;
    }
    public Room top_neighbor {
        get; private set;
    }
    public Room bottom_neighbor {
        get; private set;
    }

    List<Door> doors;

    public void SetTopNeighbor(Room r) {
        top_neighbor = r;
        r.bottom_neighbor = this;
    }
    public void SetBottomNeighbor(Room r) {
        bottom_neighbor = r;
        r.top_neighbor = this;
    }
    public void SetLeftNeighbor(Room r) {
        left_neighbor = r;
        r.right_neighbor = this;
    }
    public void SetRightNeighbor(Room r) {
        right_neighbor = r;
        r.left_neighbor = this;
    }

    public IntTuple position {
        get; private set;
    }

    bool entered = false, discovered = false;

    public RoomLayout room_layout {
        get; private set;
    }

    void Awake() {
        doors = new List<Door>();
    }

    public void CreateDoorsAndWalls() {
        Instantiate(floor, transform, false);
        GameObject new_wall;
        if (left_neighbor != null) {
            GameObject new_door = Instantiate(left_neighbor.door);
            new_door.transform.SetParent(transform);
            new_door.transform.localPosition = new Vector3(-room_width / 2, 0, 0);
            new_door.GetComponent<Door>().SetRooms(left_neighbor, this);
            doors.Add(new_door.GetComponent<Door>());

            new_wall = Instantiate(short_door_wall);
        } else {
            new_wall = Instantiate(short_wall);
        }
        new_wall.transform.SetParent(transform);
        new_wall.transform.localPosition = new Vector3(-room_width / 2, 0, 0);
        new_wall.transform.localRotation = Quaternion.Euler(0, 0, new_wall.transform.eulerAngles.z + 180);

        if (right_neighbor != null) {
            GameObject new_door = Instantiate(right_neighbor.door);
            new_door.transform.SetParent(transform);
            new_door.transform.eulerAngles = new Vector3(0,0,180);
            new_door.transform.localPosition = new Vector3(room_width / 2, 0, 0);
            new_door.GetComponent<Door>().SetRooms(right_neighbor, this);
            doors.Add(new_door.GetComponent<Door>());

            new_wall = Instantiate(short_door_wall);
        } else {
            new_wall = Instantiate(short_wall);
        }
        new_wall.transform.SetParent(transform);
        new_wall.transform.localPosition = new Vector3(room_width / 2, 0, 0);

        if (bottom_neighbor != null) {
            GameObject new_door = Instantiate(bottom_neighbor.door);
            new_door.transform.SetParent(transform);
            new_door.transform.eulerAngles = new Vector3(0, 0, 90);
            new_door.transform.localPosition = new Vector3(0, -room_height / 2, 0);
            new_door.GetComponent<Door>().SetRooms(bottom_neighbor, this);
            doors.Add(new_door.GetComponent<Door>());

            new_wall = Instantiate(long_door_wall);
        } else {
            new_wall = Instantiate(long_wall);
        }
        new_wall.transform.SetParent(transform);
        new_wall.transform.localPosition = new Vector3(0, -room_height / 2, 0);

        if (top_neighbor != null) {
            GameObject new_door = Instantiate(top_neighbor.door);
            new_door.transform.SetParent(transform);
            new_door.transform.eulerAngles = new Vector3(0, 0, 270);
            new_door.transform.localPosition = new Vector3(0, room_height / 2, 0);
            new_door.GetComponent<Door>().SetRooms(top_neighbor, this);
            doors.Add(new_door.GetComponent<Door>());

            new_wall = Instantiate(long_door_wall);
        } else {
            new_wall = Instantiate(long_wall);
        }
        new_wall.transform.SetParent(transform);
        new_wall.transform.localPosition = new Vector3(0, room_height / 2, 0);
        new_wall.transform.localRotation = Quaternion.Euler(0, 0, new_wall.transform.eulerAngles.z + 180);
    }

    public void SetLayout(RoomLayout new_layout) {
        if (room_layout == null && new_layout.GetComponent<RoomLayout>() != null) {
            new_layout.transform.SetParent(transform, false);
            new_layout.transform.localPosition = Vector3.zero;
            room_layout = new_layout.GetComponent<RoomLayout>();
        }
    }

    public void SetLayout(GameObject layout) {
        if (room_layout == null && layout.GetComponent<RoomLayout>() != null) {
            GameObject new_layout = Instantiate(layout, transform, false);
            new_layout.transform.localPosition = Vector3.zero;
            room_layout = new_layout.GetComponent<RoomLayout>();
        }
    }

    public void SetRoomActive(bool active) {

        foreach (Door d in doors) {
            d.SetEnabled(active);
            d.SetClosedDoor(active);
        }
        if (active) {
            if (!discovered) {
                discovered = true;
                UIController.instance.map.AddRoom(position, room_layout.room_type);
            }
            if (!entered) {
                if (top_neighbor && !top_neighbor.discovered) {
                    UIController.instance.map.AddRoom(top_neighbor.position, top_neighbor.room_layout.room_type);
                    top_neighbor.discovered = true;
                }
                if (left_neighbor && !left_neighbor.discovered) {
                    UIController.instance.map.AddRoom(left_neighbor.position, left_neighbor.room_layout.room_type);
                    left_neighbor.discovered = true;
                }
                if (right_neighbor && !right_neighbor.discovered) {
                    UIController.instance.map.AddRoom(right_neighbor.position, right_neighbor.room_layout.room_type);
                    right_neighbor.discovered = true;
                }
                if (bottom_neighbor && !bottom_neighbor.discovered) {
                    UIController.instance.map.AddRoom(bottom_neighbor.position, bottom_neighbor.room_layout.room_type);
                    bottom_neighbor.discovered = true;
                }
            }
            StartCoroutine(UnlockWhenEnemiesDead());
        }
        room_layout.SetRoomEnabled(active);
    }
    IEnumerator UnlockWhenEnemiesDead() {
        while (!room_layout.can_unlock) {
            yield return null;
        }
        foreach (Door d in doors) {
            d.SetClosedDoor(false);
        }
    }

    public void SetPosition(IntTuple pos) {
        position = pos;
    }

}
