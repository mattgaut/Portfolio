using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefBossHandler : EnemyHandler {

    [SerializeField] float max_time_between_attacks;
    [SerializeField] float min_time_between_attacks;
    float last_attack = 0;
    float next_attack;
    float quick_next_attack;

    bool hands_hit_table, force_attack;
    [SerializeField] float pound_rain_length, pound_rain_drop_frequency;
    [SerializeField] GameObject rain_object;
    [SerializeField] Transform far_left_rain, far_right_rain;

    bool cleave_over;
    [SerializeField] Attack cleave_attack;
    [SerializeField] Vector3 cleaver_knockback;

    bool soup_over;
    [SerializeField] Attack soup_attack;
    [SerializeField] Buff soup_debuff;

    bool player_on_table;

    protected override void Ini() {
        base.Ini();
        cleave_attack.SetOnHit(CleaverOnHit);
        soup_attack.SetOnHit(SoupOnHit);
        enemy.SetDieEvent(DieEvent);
    }

    protected override void Update() {
        base.Update();
        last_attack += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            player_on_table = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            player_on_table = false;
        }
    }

    protected override IEnumerator AIRoutine() {
        while (enemy.alive && active) {
            if (force_attack) {
                float rand = Random.Range(0, 1f);
                if (rand < .45f) {
                    yield return Pound();
                } else if (rand < .9f) {
                    yield return Soup();
                } else {
                    yield return Soup();
                    yield return Pound();
                }
                yield return Pound();
            } else if (last_attack >= next_attack) {
                if (player_on_table) {
                    if (Random.Range(0, 1f) < 0.75) {
                        yield return Cleaver();
                    } else {
                        yield return Soup();
                    }
                } else if (Random.Range(0, 1f) < 0.6) {
                    yield return Pound();
                } else {
                    yield return Soup();
                }
            } else {
                if (player_on_table && last_attack >= quick_next_attack) {
                    if (Random.Range(0, 1f) < 0.75) {
                        yield return Cleaver();
                    } else {
                        yield return Soup();
                    }
                } else {
                    yield return null;
                }
            }
        }
    }

    IEnumerator Soup() {
        soup_over = false;
        force_attack = false;
        enemy.animator.SetTrigger("Soup");
        while (!soup_over) {
            yield return null;
        }
        Attacked();
    }
    void SoupOnHit(IDamageable hit, Attack hit_by) {
        enemy.DealDamage(enemy.power, hit);
        ICombatant comb = hit as ICombatant;
        if (comb != null) soup_debuff.ApplyTo(comb, true);
    }

    IEnumerator Cleaver() {
        cleave_over = false;
        enemy.animator.SetTrigger("Cleaver");
        while (!cleave_over) {
            yield return null;
        }
        Attacked();
    }

    void CleaverOnHit(IDamageable hit, Attack hit_by) {
        enemy.DealDamage(enemy.power * 2, hit);
        hit.TakeKnockback(enemy, cleaver_knockback);
        force_attack = Random.Range(0, 1f) > 0.5;
    }

    IEnumerator Pound() {
        force_attack = false;
        hands_hit_table = false;
        enemy.animator.SetTrigger("Pound");
        while (hands_hit_table != true) {
            yield return null;
        }
        yield return Rain(pound_rain_length);
        Attacked();
    }

    IEnumerator Rain(float length) {
        float time = length;
        float frequency = 0;
        while (time > 0) {
            time -= Time.deltaTime;
            frequency -= Time.deltaTime;
            if (frequency <= 0) {
                frequency += pound_rain_drop_frequency;
                DropObject();
            }
            yield return null;
        }
    }

    void DropObject() {
        GameObject new_projectile = Instantiate(rain_object);
        new_projectile.transform.position = far_left_rain.transform.position;
        new_projectile.transform.position += new Vector3(Random.Range(0, far_right_rain.transform.position.x - far_left_rain.transform.position.x), 0f, 0f);

        Attack proj = new_projectile.GetComponent<Attack>();
        proj.SetOnHit((hit, attack) => enemy.DealDamage(enemy.power, hit));
    }

    void Attacked() {
        last_attack = 0;
        next_attack = Random.Range(min_time_between_attacks, max_time_between_attacks);
        quick_next_attack = Mathf.Pow(Random.Range(0f, 0.9f), 2) * next_attack;
    }

    IEnumerator DieEvent() {
        StopCoroutine(ai_routine);
        enemy.animator.Rebind();
        enemy.animator.transform.SetParent(null);
        enemy.animator.SetTrigger("Dead");
        FindObjectOfType<CameraFollow>().LerpToTransform(target.transform, 1f);
        yield return null;
    }

    public void AnimPoundHandsHitTable() {
        hands_hit_table = true;
    }

    public void AnimCleaveHitboxActive() {
        cleave_attack.Enable();
    }

    public void AnimCleaveHitboxInactive() {
        cleave_attack.Disable();
    }

    public void AnimCleaveOver() {
        cleave_over = true;
    }

    public void AnimSoupHitboxActive() {
        soup_attack.Enable();
    }

    public void AnimSoupHitboxInactive() {
        soup_attack.Disable();
    }

    public void AnimSoupOver() {
        soup_over = true;
    }
}
