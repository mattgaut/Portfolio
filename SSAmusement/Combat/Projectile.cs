using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    [SerializeField] LayerMask break_mask, always_break_mask;
    [SerializeField] float speed, ignore_wall_timer, max_lifetime;

    [SerializeField] protected Vector3 base_direction;

    [SerializeField] ParticleSystem particles;

    Animator anim;
    Rigidbody2D rb;
    protected Vector3 move;
    float timer = 0;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update() {
        ignore_wall_timer -= Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > max_lifetime && max_lifetime != 0) {
            Explode();
        }
    }

    private void FixedUpdate() {
        Turn();
        move = (transform.localRotation * base_direction * speed * Time.deltaTime);
        rb.MovePosition(transform.position + move);
    }

    protected virtual void Turn() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (ignore_wall_timer <= 0 && ((1 << collision.gameObject.layer & break_mask) != 0)) {
            Explode();
        } else if ((1 << collision.gameObject.layer & always_break_mask) != 0) {
            Explode();
        }
    }

    public virtual void Explode() {
        speed = 0;
        if (particles) {
            particles.Stop();
            particles.gameObject.transform.SetParent(null);
        }
        anim.SetTrigger("Explode");
    }

    public void EndLife() {
        Destroy(gameObject);
    }
}
