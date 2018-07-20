using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_Heal : TriggeredAbility {

    [SerializeField]
    int number;

    public override string description {
        get {
            if (number > 0) {
                return "Gain " + Mathf.Abs(number) + " life.";
            } else {
                return "Lose " + Mathf.Abs(number) + " life.";
            }
        }
    }

    public override IEnumerator OnResolution() {
        yield return source.controller.Heal(number);
    }
}
