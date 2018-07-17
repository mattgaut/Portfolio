using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackHandler : EnemyHandler {

    [SerializeField] float time_between_attacks;
    [SerializeField] Attack attack;
    [SerializeField] float attack_range;
    [SerializeField] Vector3 attack_knockback;
    [SerializeField] float close_distance;

    bool attack_over;
    float last_attack;

    protected override void Ini() {
        attack.SetOnHit((hit, attack) => AttackOnHit(hit));
    }

    protected override void Update() {
        base.Update();
        last_attack += Time.deltaTime;
    }

    protected override void Deactivate() {
        base.Deactivate();
        attack_over = true;
        enemy.animator.SetBool("Mad", false);
    }

    protected override IEnumerator AIRoutine() {
        while (active) {
            if (!CanHunt()) {
                yield return Wander();
            } else {
                yield return Hunt();
            }
        }
    }

    IEnumerator Wander() {
        float direction;
        if (collision_info.left) {
            direction = 1;
        } else if (collision_info.right) {
            direction = -1;
        } else {
            direction = Random.Range(-1, 2);
        }
        float wander_length = Random.Range(0.5f, 2f);
        while (!StopMoving(direction) && wander_length > 0) {
            wander_length -= Time.deltaTime;
            input.x = direction * enemy.speed;
            if (CanHunt()) {
                break;
            }
            yield return null;
        }
    }

    IEnumerator Hunt() {
        enemy.animator.SetBool("Mad", true);
        while (CanHunt()) {
            float distance = Vector2.Distance(target.transform.position, transform.position);
            if (last_attack > time_between_attacks && distance < attack_range) { // and in range
                yield return Attack();
            } else if (Mathf.Abs(target.transform.position.x - transform.position.x) > close_distance) {
                yield return WalkTowardsTarget();
            } else {
                Face(target.transform.position.x - transform.position.x);
                yield return null;
            }
        }
        enemy.animator.SetBool("Mad", false);
    }

    IEnumerator Attack() {
        Face(target.transform.position.x - transform.position.x);
        can_flip = false;
        last_attack = 0;
        enemy.animator.SetTrigger("Attack");

        input.x = 0;

        attack_over = false;
        while (!attack_over) {
            yield return null;
        }
        yield return null;
        can_flip = true;
    }

    IEnumerator WalkTowardsTarget() {
        float direction = target.transform.position.x - transform.position.x;
        if (!StopMoving(direction)) {
            input.x = Mathf.Sign(direction) * enemy.speed;
        }
        yield return null;
        input.x = 0;
    }

    protected override bool StopMoving(float direction) {
        if (direction < 0) {
            return collision_info.left || collision_info.hanging_left || base.StopMoving(direction);
        } else if (direction > 0) {
            return collision_info.right || collision_info.hanging_right || base.StopMoving(direction);
        }
        return base.StopMoving(direction);
    }

    protected virtual void AttackOnHit(IDamageable c) {
        enemy.DealDamage(enemy.power, c);
        if (attack_knockback != Vector3.zero) {
            Vector3 real_knockback = attack_knockback;
            if (target.transform.position.x < transform.position.x) {
                real_knockback.x *= -1;
            }
            target.TakeKnockback(enemy, real_knockback);
            c.TakeKnockback(enemy, real_knockback);
        }
    }

    public void AnimDisableHitbox() {
        attack.Disable();
    }
    public void AnimAttackOver() {
        attack_over = true;
    }
    public void AnimEnableHitbox() {
        attack.Enable();
    }
}
