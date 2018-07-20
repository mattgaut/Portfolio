using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GETB_GiveMassBuff : GlobalETB {

    [SerializeField]
    int attack, toughness;
    [SerializeField]
    EffectDuration duration;
    [SerializeField]
    bool another;

    EffectHolder.StatBuff b;

    void Awake() {
        b = new EffectHolder.StatBuff(duration, attack, toughness);
    }

    public override string description {
        get { return "Give creatures you contol +"+attack+"/+"+toughness + "."; }
    }

    public override IEnumerator OnResolution() {
        yield return null;
        foreach (SpellPermanent sp in source.controller.field.cards.OfType<SpellPermanent>()) {
            if (!another || sp != source) {
                sp.effects.AddEffect(b);
            }
        }
    }
}
