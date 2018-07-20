using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_L_DamageTarget : LocalTargetedTriggeredAbility {
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

    public override bool TrueCanTarget(ITargetable t) {
        if (t as IDamageable == null) {
            return false;
        }
        return base.TrueCanTarget(t);
    }
}
