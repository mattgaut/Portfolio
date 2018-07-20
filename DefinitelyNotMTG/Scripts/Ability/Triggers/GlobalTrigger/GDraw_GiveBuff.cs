using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDraw_GiveBuff : GlobalDraw {

    [SerializeField]
    int attack, toughness;
    [SerializeField]
    EffectDuration duration;

    public override string description {
        get {
            return "Gain +" + attack + "/+" + toughness + (duration == EffectDuration.eot ? " until the end of the turn." : ".");
        }
    }

    public override IEnumerator OnResolution() {
        yield return ApplyBuff();
    }

    IEnumerator ApplyBuff() {
        source.effects.AddEffect(new EffectHolder.StatBuff(duration, attack, toughness));
        yield return null;
    }
}
