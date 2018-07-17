using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiHitAttack : Attack {

    [SerializeField] float time_between_hits;

    protected override bool HitCondition(IDamageable d) {
        if (hit_objects.ContainsKey(d)) {
            return time - hit_objects[d] > time_between_hits;
        } else {
            return true;
        }
    }
}
