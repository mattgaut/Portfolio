using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MassDamage : Spell {
    [SerializeField]
    int damage;
    [SerializeField]
    bool yours, theirs;
    [SerializeField]
    List<MajorCardType> affects;

    public override IEnumerator OnResolution() {
        if (yours) {
            foreach (SpellCombatant sp in controller.field.cards.OfType<SpellCombatant>()) {
                if (affects.Contains(sp.major_card_type))
                    DealDamage(sp, damage);
            }
        }
        if (theirs) {
            foreach (SpellCombatant sp in GameManager.instance.OtherPlayer(controller).field.cards.OfType<SpellCombatant>()) {
                if (affects.Contains(sp.major_card_type))
                    DealDamage(sp, damage);
            }
        }
        yield return null;
    }
}
