using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Projectile {

    [SerializeField] Transform target;
    [SerializeField] float turn_speed;

    public void SetTarget(Transform trans) {
        target = trans;
    }

    protected override void Turn() {
        Vector3 current_trajectory = transform.rotation * base_direction;
        float angle = Vector3.SignedAngle(current_trajectory, target.transform.position - transform.position, Vector3.forward);
        if (turn_speed * Time.deltaTime > angle) {
            transform.rotation *= Quaternion.Euler(0, 0, (angle * Time.deltaTime));
        } else {
            transform.rotation *= Quaternion.Euler(0, 0, (Mathf.Sign(angle) * turn_speed * Time.deltaTime));
        }
    }

    public override void Explode() {
        base.Explode();
        turn_speed = 0;
    }
}
