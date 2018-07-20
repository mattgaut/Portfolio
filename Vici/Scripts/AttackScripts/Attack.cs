using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour {

    [SerializeField]
    protected LayerMask hits_on;
    [SerializeField]
    protected bool hitbox_enabled;

    [SerializeField]
    bool check_stay;

    public delegate void OnHit(IDamageable d);
    protected OnHit onHit;

    public void SetOnHit(OnHit on) {
        onHit = on;
    }
    public virtual void SetHitBoxEnabled(bool enabled) {
        hitbox_enabled = enabled;
        GetComponent<Collider2D>().enabled = enabled;
        StartCoroutine(Nudge());
    }

    IEnumerator Nudge() {
        transform.position += Vector3.up * .001f;
        yield return null;
        transform.position -= Vector3.up * .001f;
    }

    protected virtual void Awake() {

    }

    protected virtual void OnTriggerEnter2D(Collider2D coll) {
        if (hitbox_enabled && coll != null && (1 << coll.gameObject.layer & hits_on.value) != 0) {
            IDamageable hit_obj = coll.gameObject.GetComponent<IDamageable>();
            TriggerEnter(hit_obj);
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D coll) {
        if (hitbox_enabled && coll != null && (1 << coll.gameObject.layer & hits_on.value) != 0) {
            IDamageable hit_obj = coll.gameObject.GetComponent<IDamageable>();
            TriggerExit(hit_obj);
        }
    }
    protected abstract void TriggerEnter(IDamageable hit_obj);
    protected virtual void TriggerExit(IDamageable hit_obj) { }

    protected virtual void OnEnable() {
        StartCoroutine(Nudge());
    }

}
