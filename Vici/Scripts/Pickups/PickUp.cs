using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D coll) {
        if ((1 << coll.gameObject.layer & 1 << LayerMask.NameToLayer("Player")) != 0) {
            PickUpObject(coll.gameObject.GetComponent<PlayerCharacter>());
        }
    }

    protected virtual void PickUpObject(PlayerCharacter pc) {

        PickedUp();
    }

    void PickedUp() {
        Destroy(gameObject);
    }
}
