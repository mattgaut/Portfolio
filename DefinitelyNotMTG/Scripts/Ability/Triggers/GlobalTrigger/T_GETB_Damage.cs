using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_GETB_Damage : T_GlobalETB {

    [SerializeField]
    int damage;

    public override string description {
        get { return "Deal " + damage + " to target(s)."; }
    }

    public override IEnumerator OnResolution() {
        foreach (IDamageable d in targets) {
            source.DealDamage(d, damage);
        }
        yield return null;
    }

    public override string TargetingDescription() {
        return "Select target(s) to deal " + damage + " damage to.";
    }
}
