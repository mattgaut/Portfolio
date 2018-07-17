using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Enemy))]
public abstract class EnemyHandler : MonoBehaviour {

    CharacterController cont;
    [SerializeField] GameObject flip_object;
    [SerializeField] Transform line_of_sight_origin;
    [SerializeField] LayerMask line_of_sight_blocking_mask;

    [SerializeField] [Range(0, 20)] float jump_height = 4;
    [SerializeField] [Range(0, 5)] float time_to_jump_apex = .4f;
    [SerializeField] bool no_gravity, need_line_of_sight;

    [SerializeField] float _aggro_range;

    [SerializeField] protected bool bump_damage;
    [SerializeField] protected Vector3 bump_knockback;
    float bump_cooldown = 0.25f;
    float last_bump;

    float acceleration_grounded = 0f;
    float acceleration_airborne = 0f;

    float gravity;
    float jump_velocity;
    float x_smooth;

    bool knocked_back_last_frame;

    Vector3 velocity, gravity_force;

    protected Coroutine ai_routine;

    Coroutine drop_routine;

    protected bool flipped { get; private set; }
    protected bool can_flip = true;

    protected bool active { get; private set; }
    protected float aggro_range { get { return _aggro_range; } }
    protected CharacterController.CollisionInfo collision_info { get { return cont.collisions; } }

    protected Enemy enemy { get; private set; }

    protected Player target;
    protected Vector2 input;

    public void SetActive(bool active) {
        if (this.active != active) {
            this.active = active;
            if (active) {
                Activate();
            } else {
                Deactivate();
            }
        }
    }
    protected bool CanHunt() {
        return CustomCanHunt() && Vector2.Distance(target.transform.position, transform.position) <= aggro_range && (!need_line_of_sight || HasLineOfSight());
    }
    protected virtual bool CustomCanHunt() {
        return true;
    }

    protected virtual bool HasLineOfSight() {
        RaycastHit2D hit = Physics2D.Linecast(line_of_sight_origin.position, target.center_mass.position, line_of_sight_blocking_mask);
        if (hit) {
            hit = Physics2D.Linecast(line_of_sight_origin.position, target.head.position, line_of_sight_blocking_mask);
            if (hit) {
                hit = Physics2D.Linecast(line_of_sight_origin.position, target.feet.position, line_of_sight_blocking_mask);
            }
        }
        return !hit;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (bump_damage && collision.gameObject.layer == LayerMask.NameToLayer("Player") && last_bump > bump_cooldown) {
            last_bump = Time.deltaTime;
            Bump();
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (bump_damage && collision.gameObject.layer == LayerMask.NameToLayer("Player") && last_bump > bump_cooldown) {
            last_bump = Time.deltaTime;
            Bump();
        }
    }

    protected virtual void Bump() {
        enemy.DealDamage(BumpDamage(), target);
        if (bump_knockback != Vector3.zero) {
            Vector3 real_knockback = bump_knockback;
            if (target.transform.position.x < transform.position.x) {
                real_knockback.x *= -1;
            }
            target.TakeKnockback(enemy, real_knockback);
        }
    }

    protected void Awake() {
        cont = GetComponent<CharacterController>();
        enemy = GetComponent<Enemy>();

        gravity = -(2 * jump_height) / (time_to_jump_apex * time_to_jump_apex);
        if (no_gravity) gravity = 0;
        jump_velocity = time_to_jump_apex * Mathf.Abs(gravity);
    }

    protected void Start() {
        Tester tester;
        if (tester = FindObjectOfType<Tester>()){
            target = tester.player;
        }
        Ini();
    }

    protected virtual void Ini() {}

    protected virtual float BumpDamage() {
        return enemy.power / 2;
    }

    protected virtual void Activate() {
        ai_routine = StartCoroutine(AIRoutine());
    }
    protected virtual void Deactivate() {
        StopAllCoroutines();
        input = Vector2.zero;
        enemy.animator.Rebind();
        enemy.health.current = enemy.health;
        can_flip = true;
    }

    protected virtual void Update() {
        last_bump += Time.deltaTime;
        Move();
    }

    void Move() {
        Vector3 movement = Vector3.zero;
        if (cont.collisions.above || cont.collisions.below) {
            velocity.y = 0;
            gravity_force.y = 0;
        }
        gravity_force.y += gravity * Time.deltaTime;

        if (!enemy.knocked_back) {
            knocked_back_last_frame = false;
            if (input.y > 0 && cont.collisions.below) {
                velocity.y = jump_velocity;
            }
            if (input.y < 0 && drop_routine == null) {
                drop_routine = StartCoroutine(DropRoutine());
            }

            velocity.x = input.x;
            movement = (velocity + gravity_force) * Time.deltaTime;
            Face(movement.x);
        } else {
            if (knocked_back_last_frame == false) gravity_force = Vector3.zero;
            knocked_back_last_frame = true;
            velocity.y = 0;
            movement = (gravity_force + enemy.knockback_force) * Time.deltaTime;
        }

        cont.Move(movement);

        if (enemy.knocked_back && (cont.collisions.left || cont.collisions.right)) {
            enemy.CancelXKnockBack();
        }
        if (enemy.knocked_back && cont.collisions.above) {
            enemy.CancelYKnockBack();
        }
        if (enemy.knocked_back && cont.collisions.below) {
            enemy.CancelKnockBack();
        }

        input = Vector2.zero;
    }

    IEnumerator DropRoutine() {
        cont.RemovePlatformFromMask();
        float delay = .15f;
        while (delay > 0) {
            yield return null;
            delay -= Time.deltaTime;
        }
        while (Input.GetKey(KeyCode.S)) {
            yield return null;
        }
        cont.AddPlatformToMask();
        drop_routine = null;
    }

    protected abstract IEnumerator AIRoutine();

    public void Face(float i) {
        if (!can_flip) return;
        if (i > 0) {
            flip_object.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            flipped = true;
        } else if (i < 0) {
            flip_object.transform.localRotation = Quaternion.Euler(0, 0, 0);
            flipped = false;
        }
    }
    protected bool StopMoving(int direction) {
        return StopMoving(direction);
    }
    protected virtual bool StopMoving(float direction) {
        return collision_info.past_max;
    }
}
