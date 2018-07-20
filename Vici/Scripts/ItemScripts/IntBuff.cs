using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntBuff : Item {

    [SerializeField]
    float flat;
    [SerializeField]
    float multi;

    Stat.StatBuff applied_buff;

    protected override void OnDrop(PlayerCharacter pc) {
        pc.intelligence.RemoveBuff(applied_buff);
    }

    protected override void OnPickup(PlayerCharacter pc) {
        applied_buff = new Stat.StatBuff(pc.intelligence, flat, multi == 0 ? 1 : multi);

        pc.intelligence.ApplyBuff(applied_buff);
    }
}
