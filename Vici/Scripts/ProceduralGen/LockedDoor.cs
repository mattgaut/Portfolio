using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour {

    [SerializeField]
    Door d;

    [SerializeField]
    bool boss_key_needed;

    void OnCollisionEnter2D(Collision2D coll) {
        if (!d.closed && d.locked && coll.gameObject.layer == LayerMask.NameToLayer("Player")) {

            if (boss_key_needed) {
                if (coll.gameObject.GetComponent<PlayerCharacter>().inventory.UseBossKey()) {
                    d.SetLocked(false);
                }
            } else {
                if (coll.gameObject.GetComponent<PlayerCharacter>().inventory.UseKey()) {
                    d.SetLocked(false);
                }
            }
        }
    }
}
