using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GEOT_DrawCard : GlobalEOT {

    [SerializeField]
    int amount;

    public override IEnumerator OnResolution() {
        yield return GameManager.instance.DrawCard(source.controller, amount);
    }
}
