﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GETB_DamageOpponent : GlobalETB {
    [SerializeField]
    int damage;

    public override string description {
        get { return "Deal " + damage + " to your opponent."; }
    }

    public override IEnumerator OnResolution() {
        yield return null;
        source.DealDamage(GameManager.instance.OtherPlayer(source.controller), damage);
    }
}
