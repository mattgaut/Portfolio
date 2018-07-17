using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefCarrotHandler : EnemyHandler {

    [SerializeField] GameObject carrot;
    [SerializeField] Transform carrot_spawn_transform;

    float time_between_throws = 1.8f;
    float last_throw;

    bool throw_released;

    protected override void Update() {
        base.Update();
        last_throw += Time.deltaTime;
    }

    protected override void Activate() {
        base.Activate();
        last_throw = 0;
    }
    protected override void Deactivate() {
        base.Deactivate();
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
            if (last_throw > time_between_throws) {
                yield return ThrowCarrot();
            } else if (Vector2.Distance(target.transform.position, transform.position) < 3) {
                yield return WalkAwayFromTarget();
            } else if (Vector2.Distance(target.transform.position, transform.position) > 6) {
                yield return WalkTowardsTarget();
            } else {
                Face(target.transform.position.x - transform.position.x);
                yield return null;
            }
        }

        enemy.animator.SetBool("Mad", false);
    }

    IEnumerator ThrowCarrot() {
        throw_released = false;
        Face(target.transform.position.x - transform.position.x);
        last_throw = 0;
        enemy.animator.SetTrigger("Attack");

        while (!throw_released) {
            Face(target.transform.position.x - transform.position.x);
            yield return null;
        }
        yield return null;
    }

    IEnumerator WalkTowardsTarget() {
        float direction = target.transform.position.x - transform.position.x;
        if (!StopMoving(direction)) {
            input.x = Mathf.Sign(direction) * enemy.speed;
        }
        yield return null;
        input.x = 0;
    }
    IEnumerator WalkAwayFromTarget() {
        float direction = -target.transform.position.x + transform.position.x;
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

    public void AnimReleaseThrow() {
        throw_released = true;

        GameObject new_carrot = Instantiate(carrot);
        new_carrot.transform.position = carrot_spawn_transform.position;
        float angle = 0;
        Vector3 to_target = target.transform.position - transform.position;
        if (to_target.x < 0) {
            angle = Vector2.SignedAngle(Vector2.left, to_target);
            angle = Mathf.Clamp(angle, -45, 45);
        } else {
            angle = Vector2.SignedAngle(Vector2.right, to_target) + 180;
            angle = Mathf.Clamp(angle, 135, 225);
        }
        new_carrot.transform.rotation = Quaternion.Euler(0, 0, angle);

        new_carrot.GetComponent<Attack>().SetOnHit((hit, attack) => enemy.DealDamage(enemy.power, hit));
    }
}
