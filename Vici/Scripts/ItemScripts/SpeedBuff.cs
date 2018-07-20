using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : Item {

    [SerializeField]
    float flat;
    [SerializeField]
    float multi;

    Stat.StatBuff applied_buff;

    protected override void OnDrop(PlayerCharacter pc) {
        pc.speed.RemoveBuff(applied_buff);
    }

    protected override void OnPickup(PlayerCharacter pc) {
        applied_buff = new Stat.StatBuff(pc.speed, flat, multi == 0 ? 1 : multi);

        pc.speed.ApplyBuff(applied_buff);
    }
}
