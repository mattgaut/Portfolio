using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickEffectBuffDot : TickEffectBuff {

    [SerializeField] float damage;
    protected override void TickEffect(ICombatant stat_entity) {
        stat_entity.TakeDamage(damage, stat_entity);
    }
}
