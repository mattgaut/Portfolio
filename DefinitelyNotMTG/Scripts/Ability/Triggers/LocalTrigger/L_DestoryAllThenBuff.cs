using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_DestoryAllThenBuff : L_DestroyAll {

    public override IEnumerator OnResolution() {
        yield return base.OnResolution();

        int count = 0;
        foreach (SpellPermanent sp in to_destroy) {
            if (sp.zone != Zone.field) {
                count++;
            }
        }

        source.effects.AddEffect(new EffectHolder.StatBuff(EffectDuration.permanent, count, count));
    }
}
