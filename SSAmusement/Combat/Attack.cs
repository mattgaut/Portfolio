using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Attack : MonoBehaviour {

    public delegate void OnHit(IDamageable hit, Attack hit_by);

    protected Collider2D hitbox;
    protected Dictionary<IDamageable, float> hit_objects;
    protected float time {
        get; private set;
    }
    protected OnHit on_hit {
        get; private set;
    }

    [SerializeField] LayerMask targets;
    ICombatant source;

    protected virtual void Awake() {
        hitbox = GetComponent<Collider2D>();
        hit_objects = new Dictionary<IDamageable, float>();
        time = 0;
    }

    protected virtual void Update() {
        time += Time.deltaTime;
    }

    public void SetOnHit(OnHit _on_hit) {
        on_hit = _on_hit;
    }
    public void SetSource(ICombatant _source) {
        source = _source;
    }
    public virtual void Enable() {
        hit_objects.Clear();
        StartCoroutine(BumpHitbox());
        time = 0;
        hitbox.enabled = true;
    }
    public virtual void Disable() {
        hitbox.enabled = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & targets) != 0) {
            ConfirmHit(collision.gameObject.GetComponentInParent<IDamageable>());
        }
    }
    public virtual void OnTriggerStay2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & targets) != 0) {
            ConfirmHit(collision.gameObject.GetComponentInParent<IDamageable>());
        }
    }

    protected abstract bool HitCondition(IDamageable d);

    void ConfirmHit(IDamageable d) {
        if (!d.invincible && HitCondition(d)) {
            LogHit(d);
            on_hit(d, this);
        }
    }
    void LogHit(IDamageable d) {
        if (hit_objects.ContainsKey(d)) {
            hit_objects[d] = time;
        } else {
            hit_objects.Add(d, time);
        }
    }
    IEnumerator BumpHitbox() {
        transform.position += Vector3.one * .001f;
        yield return null;
        transform.position -= Vector3.one * .001f;
    }
}
