using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMan : Enemy {

    Rigidbody2D body;

    public bool jumping {
        get; private set;
    }
    public void StopJumping() {
        jumping = false;
    }

    [SerializeField]
    bool airborne, animating;
    [SerializeField]
    GameObject laser, rotated90, rotated180, rotated270;
    [SerializeField]
    LayerMask blocks_laser;

    protected override void Start() {
        base.Start();
        body = GetComponent<Rigidbody2D>();

        laser.SetActive(true);
        laser.GetComponent<Attack>().SetOnHit(OnHit);
        laser.SetActive(false);


        rotated90.SetActive(true);
        rotated90.GetComponent<Attack>().SetOnHit(OnHit);
        rotated90.SetActive(false);


        rotated180.SetActive(true);
        rotated180.GetComponent<Attack>().SetOnHit(OnHit);
        rotated180.SetActive(false);


        rotated270.SetActive(true);
        rotated270.GetComponent<Attack>().SetOnHit(OnHit);
        rotated270.SetActive(false);
    }

    public Coroutine StartJumpAround(int jump_count = 1) {
        return StartCoroutine(JumpAround(jump_count));
    }
    IEnumerator JumpAround(int jump_count = 1) {
        for (int i = 0; i < jump_count; i++) {
            jumping = true;
            yield return Jump(Vector3.zero);
            jumping = false;
        }
    }

    public Coroutine StartJumpTowardsPlayer(PlayerCharacter pc, int jump_count = 1) {
        return StartCoroutine(JumpTowardsPlayer(pc, jump_count));
    }
    public IEnumerator JumpTowardsPlayer(PlayerCharacter pc, int jump_count = 1) {
        for (int i = 0; i < jump_count; i++) {
            jumping = true;
            yield return JumpTowards(pc);
            jumping = false;
        }
    }

    IEnumerator Jump(Vector3 direction) {
        animator.SetTrigger("Jump");
        while (!airborne) {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            yield return null;
        }
        Vector3 movement = direction.normalized * speed.value * Time.deltaTime;
        while (airborne) {
            body.MovePosition(movement + transform.position);
            yield return null;
        }
        while (animating) {
            yield return null;
        }
    }
    IEnumerator JumpTowards(PlayerCharacter towards) {
        yield return null;
        animator.SetTrigger("Jump");
        while (!airborne) {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, towards.transform.position - transform.position);
            yield return null;
        }
        Vector3 movement = (towards.transform.position - transform.position).normalized * speed.value * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, towards.transform.position - transform.position);
        while (airborne) {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, towards.transform.position - transform.position);
            body.MovePosition(movement + transform.position);
            yield return null;
        }
        while (animating) {
            yield return null;
        }
    }


    bool use_laser = false;
    public Coroutine StartFire(PlayerCharacter target) {
        return StartCoroutine(Fire(target));
    }

    IEnumerator Fire(PlayerCharacter target) {
        animator.SetTrigger("FireLaser");
        while (!use_laser) {
            Quaternion look_to = Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look_to, 90f * Time.deltaTime);
            yield return null;
        }
        laser.SetActive(true);
        rotated90.SetActive(true);
        rotated180.SetActive(true);
        rotated270.SetActive(true);
        while (use_laser) {
            yield return null;
            RaycastHit2D forward = Physics2D.Raycast(transform.rotation * (Vector2.up * 1.5f) + transform.position, transform.rotation * Vector2.up, 30f, blocks_laser);
            RaycastHit2D rotated90ray = Physics2D.Raycast(transform.rotation * Quaternion.Euler(0,0,90) * (Vector2.up * 1.5f) + transform.position, transform.rotation * Vector2.left, 30f, blocks_laser);
            RaycastHit2D rotated180ray = Physics2D.Raycast(transform.rotation * Quaternion.Euler(0, 0, 180) * (Vector2.up * 1.5f) + transform.position, transform.rotation * Vector2.down, 30f, blocks_laser);
            RaycastHit2D rotated270ray = Physics2D.Raycast(transform.rotation * Quaternion.Euler(0, 0, 270) * (Vector2.up * 1.5f) + transform.position, transform.rotation * Vector2.right, 30f, blocks_laser);
            Quaternion look_to = Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look_to, 45f * Time.deltaTime);
            if (forward.collider != null) {
                Debug.DrawLine(forward.point, transform.rotation * (Vector2.up * 1.5f) + transform.position, Color.black);
                laser.transform.localPosition = (Vector2.up * (forward.distance/2f + 1.5f));
                laser.transform.localScale = new Vector3(1, forward.distance, 1);
            }
            if (rotated90ray.collider != null) {
                Debug.DrawLine(rotated90ray.point, transform.rotation * Quaternion.Euler(0, 0, 90) * (Vector2.up * 1.5f) + transform.position, Color.red);
                rotated90.transform.localPosition = (Vector2.left * (rotated90ray.distance / 2f + 1.5f));
                rotated90.transform.localScale = new Vector3(1, rotated90ray.distance, 1);
            }
            if (rotated180ray.collider != null) {
                Debug.DrawLine(rotated180ray.point, transform.rotation * Quaternion.Euler(0, 0, 180) * (Vector2.up * 1.5f) + transform.position, Color.blue);
                rotated180.transform.localPosition = (Vector2.down * (rotated180ray.distance / 2f + 1.5f));
                rotated180.transform.localScale = new Vector3(1, rotated180ray.distance, 1);
            }
            if (rotated270ray.collider != null) {
                Debug.DrawLine(rotated270ray.point, transform.rotation * Quaternion.Euler(0, 0, 270) * (Vector2.up * 1.5f) + transform.position, Color.green);
                rotated270.transform.localPosition = (Vector2.right * (rotated270ray.distance / 2f + 1.5f));
                rotated270.transform.localScale = new Vector3(1, rotated270ray.distance, 1);
            }
        }
        laser.SetActive(false);
        rotated90.SetActive(false);
        rotated180.SetActive(false);
        rotated270.SetActive(false);
        use_laser = false;
        while (animating) {
            yield return null;
        }
    }

    public void ToggleLaser() {
        use_laser = !use_laser;
    }

    void OnHit(IDamageable d) {
        if (d.GetType().IsSubclassOf(typeof(PlayerCharacter))) {
            d.TakeDamage(1);
        }
    }
}
