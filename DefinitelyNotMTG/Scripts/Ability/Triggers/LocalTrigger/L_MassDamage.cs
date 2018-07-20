using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class L_MassDamage : TriggeredAbility {

    [SerializeField]
    int damage;
    [SerializeField]
    bool yours, theirs, another;
    [SerializeField]
    List<MajorCardType> affects;

    public override string description {
        get {
            return "Deal " + damage + " to all other creatures.";
        }
    }

    public override IEnumerator OnResolution() {
        if (yours) {
            foreach (SpellCombatant sp in source.controller.field.cards.OfType<SpellCombatant>()) {
                if (affects.Contains(sp.major_card_type) && (!another || sp != source))
                    source.DealDamage(sp, damage);
            }
        }
        if (theirs) {
            foreach (SpellCombatant sp in GameManager.instance.OtherPlayer(source.controller).field.cards.OfType<SpellCombatant>()) {
                if (affects.Contains(sp.major_card_type)) {

                    source.DealDamage(sp, damage);
                }
            }
        }
        yield return null;
    }
}
