using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaEntrance : MonoBehaviour {

    [SerializeField] BossRoom room;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            room.OnEnterArena();
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
