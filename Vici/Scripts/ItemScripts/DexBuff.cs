using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DexBuff : Item {

    [SerializeField]
    float flat;
    [SerializeField]
    float multi;

    Stat.StatBuff applied_buff;

    protected override void OnDrop(PlayerCharacter pc) {
        pc.dexterity.RemoveBuff(applied_buff);
    }

    protected override void OnPickup(PlayerCharacter pc) {
        applied_buff = new Stat.StatBuff(pc.dexterity, flat, multi == 0 ? 1 : multi);

        pc.dexterity.ApplyBuff(applied_buff);
    }
}
