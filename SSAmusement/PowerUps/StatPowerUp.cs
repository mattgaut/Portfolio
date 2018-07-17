using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPowerUp : PowerUp {
    [SerializeField] Buff buff;

    public override float length {
        get { return buff.buff_length; }
    }

    protected override void AddPowerup(Player p) {
        base.AddPowerup(p);
        buff.ApplyTo(p);

        timer = 0;
    }
}
