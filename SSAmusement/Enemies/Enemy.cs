using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICombatant {

    [SerializeField] Animator anim;
    public Animator animator {
        get { return anim; }
    }

    public Room home {
        get; private set;
    }

    [SerializeField] CapStat _health, _energy;
    [SerializeField] Stat _power, _speed, _armor;
    public CapStat health {
        get { return _health; }
    }
    public Stat power {
        get { return _power; }
    }
    public Stat armor {
        get { return _armor; }
    }

    public Stat speed {
        get { return _speed; }
    }

    public CapStat energy {
        get { return _energy; }
    }

    [SerializeField] protected bool knockback_resistant;
    float remaining_knockback_time;
    Coroutine knockback_routine;
    public Vector3 knockback_force {
        get; private set;
    }
    public bool knocked_back {
        get; private set;
    }
    public bool alive {
        get { return health.current > 0; }
    }
    public bool invincible {
        get; private set;
    }

    ICombatant last_hit_by;

    System.Func<IEnumerator> die_function;

    protected virtual void Awake() {
        health.current = health;
    }

    public void SetRoom(Room r) {
        home = r;
    }

    public float DealDamage(float damage, IDamageable target, bool trigger_on_hit = true) {
        return target.TakeDamage(damage, this);
    }

    public float TakeDamage(float damage, ICombatant source) {
        float old = health.current;
        health.current -= Mathf.Max(damage - armor, 0);
        old -= health.current;
        last_hit_by = source;
        if (health.current <= 0) {
            Die();
        }
        return old;
    }
    public void TakeKnockback(ICombatant source, Vector3 force, float length = 0.5f) {
        if (knockback_resistant) return;
        knockback_force = force;
        remaining_knockback_time = length;
        if (knockback_routine != null) {
            StopCoroutine(knockback_routine);
        }
        knockback_routine = StartCoroutine(KnockbackRoutine(force, length));
    }

    IEnumerator KnockbackRoutine(Vector3 force, float length) {
        knocked_back = true;

        while (remaining_knockback_time > 0) {
            knockback_force = force * Mathf.Pow(remaining_knockback_time / length, 0.8f);
            remaining_knockback_time -= Time.deltaTime;
            if (knocked_back == false) {
                break;
            }
            yield return null;
        }
        knockback_force = Vector3.zero;
        while (knocked_back) {
            yield return null;
        }
    }

    public void CancelKnockBack() {
        knocked_back = false;
    }
    public void CancelYKnockBack() {
        knockback_force -= Vector3.up * knockback_force.y;
    }
    public void CancelXKnockBack() {
        knockback_force -= Vector3.right * knockback_force.x;
    }

    public void KillCredit(ICombatant killed) {

    }

    public void LogBuff(Buff b) { }

    public void SetDieEvent(System.Func<IEnumerator> die_event) {
        die_function = die_event;
    }

    void Die() {
        last_hit_by.KillCredit(this);

        if (die_function != null) {
            StartCoroutine(BeforeDestroy());
        } else {
            home.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }

    IEnumerator BeforeDestroy() {
        yield return die_function();
        Destroy(gameObject);
        home.RemoveEnemy(this);
    }
}
