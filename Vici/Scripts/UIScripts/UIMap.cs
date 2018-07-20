using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMap : MonoBehaviour {

    [SerializeField]
    GameObject normal_room_view_object, boss_room_view_object, spawn_room_view_object, item_room_view_object;
    [SerializeField]
    Vector3 offset;



    Dictionary<IntTuple, UIRoomView> displayed_rooms;
    IntTuple focus;

    public void Awake() {
        displayed_rooms = new Dictionary<IntTuple, UIRoomView>();
    }


    public void AddRoom(IntTuple position, RoomType room_type) {
        GameObject to_spawn = null;
        if (room_type == RoomType.normal) {
            to_spawn = normal_room_view_object;
        } else if (room_type == RoomType.boss) {
            to_spawn = boss_room_view_object;
        } else if (room_type == RoomType.item) {
            to_spawn = item_room_view_object;
        } else if (room_type == RoomType.spawn) {
            to_spawn = spawn_room_view_object;
        }

        GameObject new_room_view = Instantiate(to_spawn, transform);
        new_room_view.transform.localPosition = new Vector3(offset.x * position.x, offset.y * position.y);
        displayed_rooms.Add(position, new_room_view.GetComponent<UIRoomView>());
    }

    public void FocusRoom(IntTuple position) {
        if (focus != null) {
            displayed_rooms[position].SetFocus(false);
        }
        focus = position;
        GetComponent<LerpTo>().LerpToPosition(-new Vector3((offset.x * position.x), (offset.y * position.y)) + transform.parent.position, 1f); 
        displayed_rooms[focus].SetFocus(true);
    }
}
