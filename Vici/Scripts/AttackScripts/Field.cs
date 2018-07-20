using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MultiHit {

    [SerializeField]
    float length;
    float timer;

    protected OnHit leave;

    void Update() {
        timer += Time.deltaTime;
        if (timer > length) {
            Destroy(gameObject);
        }
    }

    protected override void TriggerExit(IDamageable hit_obj) {
        if (hit_objects.Contains(hit_obj)) {
            hit_objects.Remove(hit_obj);
            leave(hit_obj);
        }
    }

    public virtual void SetLeave(OnHit leave) {
        this.leave = leave;
    }

    protected override void OnDisable() {
        foreach (IDamageable e in hit_objects) {
            leave(e);
        }
        base.OnDisable();
    }
}
