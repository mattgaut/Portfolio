using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureDisplay : FieldCardDisplay {

    [SerializeField]
    Text health, attack;

    [SerializeField]
    Image claw, castle;

    public override void Display(Card c) {
        base.Display(c);
        Creature card = c as Creature;
        if (card != null) {
            health.text = (card.health_remaining) + "";
            attack.text = card.attack_power + "";

            if (card.damage_taken > 0) {
                health.color = Color.red;
            } else {
                health.color = Color.black;
            }
            claw.enabled = true;
            castle.enabled = false;
        }
        Structure card2 = c as Structure;
        if (card2 != null) {
            health.text = (card2.health_remaining) + "";
            attack.text = card2.attack_power + "";

            if (card2.damage_taken > 0) {
                health.color = Color.red;
            } else {
                health.color = Color.black;
            }
            claw.enabled = false;
            castle.enabled = true;
        }
    }
}
