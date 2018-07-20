using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GETB_Heal : GlobalETB {
    [SerializeField]
    int amount;

    public override string description {
        get { return "Heal " + amount + " life."; }
    }

    public override IEnumerator OnResolution() {
        yield return null;
        source.controller.Heal(amount);
    }
}
