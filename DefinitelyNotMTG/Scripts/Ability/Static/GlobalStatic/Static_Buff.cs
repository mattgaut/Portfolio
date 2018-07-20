using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Buff : StaticAbility {

    [SerializeField]
    int attack, toughness;


    EffectHolder.Effect buff;

    public override void Awake() {
        base.Awake();
        buff = new EffectHolder.StatBuff(EffectDuration.permanent, attack, toughness);
    }

    protected override void Apply(Card c) {
        c.effects.AddEffect(buff);
    }

    protected override void UnApply(Card c) {
        c.effects.RemoveEffect(buff);
    }

}
