using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : SingleHit {

    [SerializeField]
    LayerMask breaks_on;

    [SerializeField]
    float speed, range;

    float moved;

    Vector3 direction;

    [SerializeField]
    Rigidbody2D body;

    protected void Start() {
        direction = (transform.rotation * Vector3.up).normalized;
    }

    void Update() {
        Move();
    }

    void Move() {
        body.MovePosition(transform.position + direction * speed * Time.deltaTime);
        moved += (direction * speed * Time.deltaTime).magnitude;

        if (moved > range) {
            OutOfRange();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D coll) {
        base.OnTriggerEnter2D(coll);

        if ((1 << coll.gameObject.layer & breaks_on) != 0) {
            Breaks();
        }
    }

    protected virtual void OutOfRange() {
        Destroy(gameObject);
    }

    protected virtual void Breaks() {
        Destroy(gameObject);
    }
}
