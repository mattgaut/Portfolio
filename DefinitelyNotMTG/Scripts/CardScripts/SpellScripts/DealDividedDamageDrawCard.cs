using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDividedDamageDrawCard : TargetedSpell {

    [SerializeField]
    int damage;

    protected override IEnumerator ResolveOnTargets() {
        foreach (IDamageable d in targets) {
            d.TakeDamage(this, damage/targets.Count);

            if (d as SpellCombatant != null) {
                if ((d as SpellCombatant).dead) {
                    yield return GameManager.instance.DrawCard(controller, 1);
                }
            }
        }
    }

    public override bool TrueCanTarget(ITargetable target) {
        return (target is IDamageable) && base.TrueCanTarget(target);
    }

    public override string TargetingDescription() {
        return "Select target(s) to split " + damage + " damage between.";
    }
}
