using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : PickUp {

    [SerializeField]
    int heal_value;

    protected override void PickUpObject(PlayerCharacter pc) {
        if (pc.health != pc.max_health.value) {
            pc.Heal(heal_value);

            base.PickUpObject(pc);
        }
    }
}
