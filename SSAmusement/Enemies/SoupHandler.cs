using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupHandler : MeleeAttackHandler {

    [SerializeField] Buff soup_debuff;
    protected override void AttackOnHit(IDamageable c) {
        base.AttackOnHit(c);
        ICombatant comb = c as ICombatant;
        if (comb != null) soup_debuff.ApplyTo(comb, true);
    }
}
