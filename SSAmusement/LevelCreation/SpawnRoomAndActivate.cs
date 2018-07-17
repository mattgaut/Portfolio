using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomAndActivate : MonoBehaviour {

    [SerializeField] Room to_activate;
    public void Start() {
        to_activate.SpawnRandomRoomset();
        RoomManager.instance.SetActiveRoom(to_activate);
        to_activate.SetSize(18, 10);
    }
}
