using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHitAttack : Attack {
    protected override bool HitCondition(IDamageable d) {
        return !hit_objects.ContainsKey(d);
    }
}
