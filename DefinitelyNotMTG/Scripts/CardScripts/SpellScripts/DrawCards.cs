using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : Spell {

    [SerializeField]
    int amount;

    public override IEnumerator OnResolution() {
        yield return GameManager.instance.DrawCard(controller, amount); ;
    }
}
