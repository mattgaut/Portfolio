using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetFieldCardSpell : TargetedSpell {


    [SerializeField]
    bool can_hit_players, can_hit_creatures;

    public override bool TrueCanTarget(ITargetable target) {
        Creature c = target as Creature;
        if (c != null) {
            if (c.zone != Zone.field) return can_hit_creatures;
        } else {
            Player p = target as Player;
            if (p != null) return can_hit_players;
        }
        return base.TrueCanTarget(target);
    }
}
