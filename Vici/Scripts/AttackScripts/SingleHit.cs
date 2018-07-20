using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHit : Attack {

    List<IDamageable> hit;

    protected override void Awake() {
        hit = new List<IDamageable>();
    }

    public override void SetHitBoxEnabled(bool t) {
        base.SetHitBoxEnabled(t);
        hit.Clear();
    }

    protected override void TriggerEnter(IDamageable hit_obj) {
        if (!hit.Contains(hit_obj)) {
            hit.Add(hit_obj);
            onHit(hit_obj);
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (hit != null) {
            hit.Clear();
        } else {
            hit = new List<IDamageable>();
        }
    }
}
