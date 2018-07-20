using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiveBuff : TargetedSpell {
    [SerializeField]
    protected int att_buff, def_buff;
    [SerializeField]
    EffectDuration duration;

    EffectHolder.StatBuff buff;

    protected override void Awake() {
        base.Awake();
        buff = new EffectHolder.StatBuff(duration, att_buff, def_buff);
    }

    public override IEnumerator OnResolution() {
        foreach (SpellPermanent target in targets.OfType<SpellPermanent>()) {
            target.effects.AddEffect(buff);
        }
        yield return null;
    }

    public override string TargetingDescription() {
        return "Give targets " + att_buff +"/-"+ def_buff +" until the end of the turn.";
    }
}
