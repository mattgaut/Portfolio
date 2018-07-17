using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : EnemyHandler {

    [SerializeField] bool get_mad;

    protected override void Ini() {
        base.Ini();
        SetActive(true);
    }

    protected override bool CustomCanHunt() {
        return false;
    }

    protected override IEnumerator AIRoutine() {
        if (get_mad) enemy.animator.SetTrigger("Mad");
        while (true) {
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
                yield return null;
            }
        }

    }
}
