using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory), typeof(PlayerDisplay))]
public class Player : MonoBehaviour, ICombatant {

    [SerializeField] bool god_mode;

    [SerializeField] Animator anim;
    public Animator animator {
        get { return anim; }
    }

    Inventory _inventory;
    public Inventory inventory {
        get { return _inventory; }
    }

    public PlayerDisplay player_display { get; private set; }
    public bool can_change_facing {
        get { return true; }
    }

    [SerializeField] CapStat _health, _energy;
    [SerializeField] Stat  _power, _speed, _armor;
    [SerializeField] Attack basic_attack;
    float basic_attack_time_before_active = 0.1f;
    float basic_attack_length = 0.12f;
    float basic_attack_delay = 0.8f;

    float knockback_dissipation_time;
    Coroutine knockback_routine;
    Vector3 original_knockback_force;

    bool basic_attacking;
    bool basic_attack_delay_over;
    bool can_basic_attack {
        get {
            return basic_attack_delay_over;
        }
    }

    [SerializeField] Collider2D hitbox;
    [SerializeField] float invincibility_length = 1.5f;
    Coroutine iframes;

    public bool invincible {
        get; private set;
    }

    [SerializeField] Transform _center_mass, _feet, _head;
    public Transform center_mass {
        get { return _center_mass; }
    }
    public Transform feet {
        get { return _feet; }
    }
    public Transform head {
        get { return _head; }
    }

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

    public Vector3 knockback_force {
        get; private set;
    }
    public bool knocked_back {
        get; private set;
    }
    public bool alive {
        get; private set;
    }
    public bool can_input {
        get { return alive && !knocked_back; }
    }

    public delegate void OnHitCallback(Player player, float pre_mitigation_damage, float post_mitigation_damage, IDamageable hit);
    List<OnHitCallback> on_hits;

    public delegate void OnKillCallback(Player player, ICombatant killed);
    List<OnKillCallback> on_kills;

    public delegate void OnTakeDamage(Player player, float pre_mitigation_damage, float post_mitigation_damage, ICombatant hit_by);
    List<OnTakeDamage> on_take_damages;

    void Awake() {
        _inventory = GetComponent<Inventory>();
        player_display = GetComponent<PlayerDisplay>();

        basic_attack.SetSource(this);
        basic_attack.SetOnHit(BassicAttackOnHit);
        basic_attack_delay_over = true;
        health.current = health;

        on_hits = new List<OnHitCallback>();
        on_kills = new List<OnKillCallback>();
        on_take_damages = new List<OnTakeDamage>();

        alive = true;
    }

    void Update() {
        player_display.UpdateHealthBar(health.current, health);
    }

    public float DealDamage(float damage, IDamageable target, bool trigger_on_hit = true) {
        float damage_dealt = target.TakeDamage(damage, this);
        if (damage_dealt > 0 && trigger_on_hit) {
            foreach (OnHitCallback oh in on_hits) {
                oh(this, damage, damage_dealt, target);
            }
        }
        return damage_dealt;
    }

    public float TakeDamage(float damage, ICombatant source) {
        float old = health.current;
        float post_mitigation_damage = Mathf.Max(damage - armor, 0);
        if (post_mitigation_damage > 0) {
            if (iframes != null)StopCoroutine(iframes);
            iframes = StartCoroutine(IFrames());
            health.current -= post_mitigation_damage;
            if (health.current <= 0 && !god_mode) {
                Die();
            } else {
                foreach (OnTakeDamage otd in on_take_damages) {
                    otd(this, damage, post_mitigation_damage, source);
                }
            }
        }
        return old - health.current;
    }
    public float RestoreHealth(float restore) {
        float old = health.current;
        health.current += restore;
        return health.current - old;
    }

    void Die() {
        alive = false;
        UIHandler.GameOver();
        hitbox.gameObject.SetActive(false);
        anim.SetTrigger("Death");
        anim.SetBool("Flicker", false);
        player_display.Disable();
    }

    public void TakeKnockback(ICombatant source, Vector3 force, float length = 0.5f) {
        knockback_force = force;
        knockback_dissipation_time = length;
        if (knockback_routine != null) {
            StopCoroutine(knockback_routine);
        }
        knockback_routine = StartCoroutine(KnockbackRoutine(force, length));
    }

    public void BasicAttack() {
        if (can_basic_attack) {
            StartCoroutine(BasicAttackCoroutine());
        }
    }

    void BassicAttackOnHit(IDamageable d, Attack hit_by) {
        DealDamage(power, d);
        d.TakeKnockback(this, new Vector3(5 * Mathf.Sign(d.gameObject.transform.position.x - transform.position.x), 5,0));
    }

    IEnumerator BasicAttackCoroutine() {
        anim.SetTrigger("Attack");
        float time = 0;
        basic_attacking = true;
        basic_attack_delay_over = false;

        while (basic_attacking && time < basic_attack_time_before_active) { // wait before hitbox active
            time += Time.deltaTime;
            yield return null;
        }
        basic_attack.Enable();
        while (basic_attacking && time < basic_attack_length + basic_attack_time_before_active) { // length of basic attack
            time += Time.deltaTime;
            yield return null;
        }
        basic_attack.Disable();
        basic_attacking = false;

        while (time < basic_attack_delay) { // delay between basic attacks
            time += Time.deltaTime;
            yield return null;
        }
        basic_attack_delay_over = true;
    }

    IEnumerator KnockbackRoutine(Vector3 initial_knockback_force, float length) {
        original_knockback_force = initial_knockback_force;
        knocked_back = true;
        animator.SetBool("KnockedBack", true);
        basic_attacking = false;
        while (knockback_dissipation_time > 0 && knocked_back) {
            if (original_knockback_force == Vector3.zero) {
                break;
            }
            yield return null;
            knockback_force = original_knockback_force * Mathf.Pow(knockback_dissipation_time / length, 0.8f);
            knockback_dissipation_time -= Time.deltaTime;

        }
        knockback_force = Vector3.zero;
        while (knocked_back) {
            yield return null;
        }
        animator.SetBool("KnockedBack", false);
    }

    public void CancelKnockBack() {
        knocked_back = false;
    }
    public void CancelYKnockBack() {
        original_knockback_force = new Vector3(original_knockback_force.x,0,original_knockback_force.z);
    }
    public void CancelXKnockBack() {
        original_knockback_force = new Vector3(0, original_knockback_force.y, original_knockback_force.z);
    }

    IEnumerator IFrames() {
        float time = 0;
        invincible = true;
        hitbox.enabled = false;
        anim.SetBool("Flicker", true);
        while (time < invincibility_length) {
            time += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("Flicker", false);
        invincible = false;
        hitbox.enabled = true;
    }

    public void KillCredit(ICombatant killed) {
        foreach (OnKillCallback ok in on_kills) {
            ok(this, killed);
        }
    }

    public void TrackPowerUp(PowerUp p) {
        player_display.DisplayTimedBuff(p);
    }

    public void LogBuff(Buff b) {
        player_display.DisplayTimedBuff(b);
    }

    public void AddOnKill(OnKillCallback ok) {
        on_kills.Add(ok);
    }

    public void RemoveOnKill(OnKillCallback ok) {
        on_kills.Remove(ok);
    }

    public void AddOnHit(OnHitCallback oh) {
        on_hits.Add(oh);
    }

    public void RemoveOnHit(OnHitCallback oh) {
        on_hits.Remove(oh);
    }

    public void AddTakeDamage(OnTakeDamage otd) {
        on_take_damages.Add(otd);
    }

    public void RemoveOnTakeDamage(OnTakeDamage otd) {
        on_take_damages.Remove(otd);
    }
}