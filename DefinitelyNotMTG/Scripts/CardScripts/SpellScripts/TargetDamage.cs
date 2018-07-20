using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDamage : TargetedSpell {

    [SerializeField]
    int damage;

    protected override IEnumerator ResolveOnTargets() {
        foreach (IDamageable d in targets) {
            d.TakeDamage(this, damage);
        }
        yield return null;
    }

    public override bool TrueCanTarget(ITargetable target) {
        return (target is IDamageable) && base.TrueCanTarget(target);
    }

    public override string TargetingDescription() {
        return "Select target(s) to deal " + damage + " damage to.";
    }
}
