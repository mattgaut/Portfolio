using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDies_HealPlayer : GlobalDies {

    [SerializeField]
    int heal;

    public override string description {
        get { return "Heal the controller for " + heal + "."; }
    }

    public override IEnumerator OnResolution() {
        yield return null;
        source.controller.Heal(heal);
    }
}
