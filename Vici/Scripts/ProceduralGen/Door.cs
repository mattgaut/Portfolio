using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    Room to_room;
    Room from_room;

    [SerializeField]
    Collider2D door_trigger;

    [SerializeField]
    GameObject enter_object;
    public Vector3 enter_space {
        get { return enter_object.transform.position; }
    }
    [SerializeField]
    GameObject closed_door_object;

    [SerializeField]
    Sprite locked_sprite, unlocked_sprite;

    [SerializeField]
    bool _locked;
    public bool locked {
        get { return _locked; }
    }

    public bool closed {
        get; private set;
    }

    public void Awake() {
        SetClosedDoor(false);
    }

    public void SetRooms(Room to_room, Room from_room) {
        this.to_room = to_room;
        this.from_room = from_room;
    }

    public void SetEnabled(bool enabled) {
        door_trigger.enabled = enabled;
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if (from_room) {
                LevelManager.instance.SetActiveRoom(to_room, true, this);
            }
        }
    }

    public void SetLocked(bool set) {
        _locked = set;
        if (!locked) {
            closed_door_object.GetComponent<SpriteRenderer>().sprite = unlocked_sprite;
            closed_door_object.SetActive(closed);
        }
    }

    public void SetClosedDoor(bool closed) {
        this.closed = closed;
        if (locked) {
            closed_door_object.SetActive(true);
            closed_door_object.GetComponent<SpriteRenderer>().sprite = locked_sprite;
        } else {
            closed_door_object.SetActive(closed);
            closed_door_object.GetComponent<SpriteRenderer>().sprite = unlocked_sprite;
        }
    }
}
