using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKeyPickup : PickUp {

    protected override void PickUpObject(PlayerCharacter pc) {
        pc.inventory.PickUpBossKey();

        base.PickUpObject(pc);
    }
}
