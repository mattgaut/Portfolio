using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : PickUp {

    [SerializeField]
    int count;

    protected override void PickUpObject(PlayerCharacter pc) {
        pc.inventory.PickUpKeys(count);

        base.PickUpObject(pc);
    }
}
