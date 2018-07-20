using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_KeywordBuff : StaticAbility {

    [SerializeField]
    KeywordAbility to_affect;

    EffectHolder.Effect buff;

    public override void Awake() {
        base.Awake();
        buff = new EffectHolder.KeywordBuff(EffectDuration.permanent, to_affect);
    }

    protected override void Apply(Card c) {
        c.effects.AddEffect(buff);
    }

    protected override void UnApply(Card c) {
        c.effects.RemoveEffect(buff);
    }
}
