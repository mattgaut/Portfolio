using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiveBuffAndKeywordBuff : GiveBuff {

    [SerializeField]
    KeywordAbility ability;

    EffectHolder.KeywordBuff key_buff;

    protected override void Awake() {
        base.Awake();
        key_buff = new EffectHolder.KeywordBuff(EffectDuration.eot, ability);
    }

    public override IEnumerator OnResolution() {
        yield return base.OnResolution();
        foreach (SpellPermanent p in targets.OfType<SpellPermanent>()) {
            p.effects.AddEffect(key_buff);
        }
    }

    public override string TargetingDescription() {
        return "Give targets +" + att_buff + "/+" + def_buff + " and unblockable until the end of the turn";
    }
}
