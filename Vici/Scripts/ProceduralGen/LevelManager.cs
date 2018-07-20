using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    LevelGenerator gen;

    public static LevelManager instance {
        get; private set;
    }

    public Room active_room {
        get; private set;
    }

    Dictionary<IntTuple, Room> rooms;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        LoadRooms();
    }

    void LoadRooms() {
        gen.GenerateLevel();
        rooms = gen.spawned_rooms;
        Destroy(gen);
    }
    public void SetInitialActive(Room r) {
        SetActiveRoom(r);
    }

    Coroutine routine;
    public void SetActiveRoom(Room r, bool lerp = false, Door from_door = null) {
        if (lerp && routine == null)
            routine = StartCoroutine(ChangeActiveRoom(r, from_door));
        else {
            if (active_room != null) {
                active_room.SetRoomActive(false);
            }
            active_room = r;
            active_room.SetRoomActive(true);
            UIController.instance.map.FocusRoom(r.position);
        }
    }

    IEnumerator ChangeActiveRoom(Room r, Door from_door = null) {
        if (active_room) {
            active_room.SetRoomActive(false);
        }
        UIController.instance.map.FocusRoom(r.position);
        yield return LerpPlayerAndCamera(r, from_door ? from_door.enter_space : r.transform.position);

        active_room = r;
        active_room.SetRoomActive(true);

        routine = null;
    }
    IEnumerator LerpPlayerAndCamera(Room r, Vector3 character_to_position) {
        PlayerCharacter pc = FindObjectOfType<PlayerCharacter>();
        pc.GetComponent<PlayerController>().player_has_control = false;
        pc.transform.rotation = Quaternion.LookRotation(Vector3.forward, character_to_position - pc.transform.position);
        Camera.main.GetComponent<LerpTo>().LerpToPosition(r.transform.position - Vector3.forward * 10, 1f);
        pc.GetComponent<LerpTo>().LerpToPosition(character_to_position, 1f);

        float timer = 1;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        pc.GetComponent<PlayerController>().player_has_control = true;
    }
}
