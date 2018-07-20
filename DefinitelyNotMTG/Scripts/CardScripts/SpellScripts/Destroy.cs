using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Destroy : TargetedSpell {

    public override IEnumerator OnResolution() {
        foreach (Card card in targets.OfType<Card>()) {
            yield return GameManager.instance.DestroyPermanent(card);
        }
    }

    public override string TargetingDescription() {
        return "Choose target to Destroy.";
    }
}
