using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : EnemyController {

    [SerializeField]
    GameObject projectile;

    [SerializeField]
    float noise_value;
    bool run, fire;

    int direction;
    Vector3 noise_vector;

    protected override void Start() {
        base.Start();

        direction = 1;
        StartCoroutine(Meander());

        noise_vector = Vector3.up;
    }

    protected override void Update() {
        if (!run && Vector3.Distance(target.transform.position, transform.position) < 4f) {
            run = true;
        } else if (run && Vector3.Distance(target.transform.position, transform.position) > 6f) {
            run = false;
        }
        Move();
        if (Random.Range(0, 17) == 0) {
            direction *= -1;
        }
        noise_vector = Quaternion.Euler(0, 0, direction * Random.Range(0f,15f)) * noise_vector;
    }

    protected IEnumerator Meander() {
        float shot_timer = Random.Range(0.75f, 3.5f);
        while (true) {
            yield return null;
            if (run) {
                yield return Run();
            } else {
                if (shot_timer < 0) {
                    movement = Vector3.zero;
                    yield return Fire();
                    shot_timer = Random.Range(0.75f, 3.5f);
                } else {
                    shot_timer -= Time.deltaTime;
                }
                movement = AddNoise(Vector3.zero);
            }
        }
    }

    protected IEnumerator Run() {
        while (run) {
            movement = -(target.transform.position - transform.position);
            movement = AddNoise(movement);
            yield return null;
        }
    }

    protected IEnumerator Fire() {
        fire = true;
        yield return null;
        GameObject new_proj = Instantiate(projectile, transform.position, transform.rotation);
        new_proj.GetComponent<Attack>().SetOnHit((IDamageable hit) => hit.TakeDamage(1));
        fire = false;
    }

    protected override void Move() {
        if (!character.stunned && movement != Vector3.zero)
            body.MovePosition(transform.position + (movement.normalized * character.speed.value * Time.deltaTime));
        Turn();
    }
    protected override void Turn() {
        if (!character.stunned)
            transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position);
    }

    Vector3 AddNoise(Vector3 vector) {
        return vector + (noise_vector * noise_value);
    }
}
