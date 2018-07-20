using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_DrawCards : TriggeredAbility {

    [SerializeField]
    int number;

    public override string description {
        get {
            return "Draw " + number + " card" + (number != 1 ? "s." : ".");
        }
    }

    public override IEnumerator OnResolution() {       
        yield return GameManager.instance.DrawCard(source.controller, number);
    }
}
