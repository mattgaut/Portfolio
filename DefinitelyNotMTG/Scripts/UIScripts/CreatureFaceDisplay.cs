using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureFaceDisplay : CardFaceDisplay {

    [SerializeField]
    Text stat_text;

    public override void Display(Card card) {
        base.Display(card);
        SpellCombatant creature = card as SpellCombatant;
        if (creature != null) {
            stat_text.enabled = true;
            stat_text.text = creature.attack_power + " / " + creature.toughness;
        } else {
            stat_text.enabled = false;
        }
    }
}
