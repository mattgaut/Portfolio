using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitBuff : Item {

    [SerializeField]
    float flat;
    [SerializeField]
    float multi;

    [SerializeField]
    bool heal_difference;

    Stat.StatBuff applied_buff;

    protected override void OnDrop(PlayerCharacter pc) {
        pc.vitality.RemoveBuff(applied_buff);
    }

    protected override void OnPickup(PlayerCharacter pc) {
        applied_buff = new Stat.StatBuff(pc.vitality, flat, multi == 0 ? 1 : multi);

        int previous = Mathf.RoundToInt(pc.max_health.value);
        pc.vitality.ApplyBuff(applied_buff);
        if (heal_difference) {
            pc.Heal(Mathf.RoundToInt(pc.max_health.value) - previous);
        } else {
            pc.Heal(0);
        }
    }
}
