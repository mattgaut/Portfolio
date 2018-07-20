using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrBuff : Item {

    [SerializeField]
    float flat;
    [SerializeField]
    float multi;

    Stat.StatBuff applied_buff;

    protected override void OnDrop(PlayerCharacter pc) {
        pc.strength.RemoveBuff(applied_buff);
    }

    protected override void OnPickup(PlayerCharacter pc) {
        applied_buff = new Stat.StatBuff(pc.strength, flat, multi == 0 ? 1 : multi);

        pc.strength.ApplyBuff(applied_buff);
    }
}
