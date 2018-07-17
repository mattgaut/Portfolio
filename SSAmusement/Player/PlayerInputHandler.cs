using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Player))]
public class PlayerInputHandler : MonoBehaviour {

    CharacterController cont;
    Player player;
    [SerializeField] GameObject flip_object;

    [SerializeField] [Range(0, 20)] float max_jump_height;
    [SerializeField] [Range(0, 20)] float min_jump_height;
    [SerializeField] [Range(0, 5)] float time_to_jump_apex;

    float acceleration_grounded = 0f;
    float acceleration_airborne = 0f;

    float gravity;
    float jump_velocity;
    float max_jump_hold;
    float x_smooth;

    bool jumping, knocked_back_last_frame;

    Vector3 velocity;
    Vector3 gravity_force;

    Coroutine drop_routine;

    private void Awake() {
        cont = GetComponent<CharacterController>();
        player = GetComponent<Player>();

        gravity = -(2 * min_jump_height) / (time_to_jump_apex * time_to_jump_apex);
        jump_velocity = time_to_jump_apex * Mathf.Abs(gravity);
        max_jump_hold = (max_jump_height - min_jump_height) / jump_velocity;
    }

    private void Update() {
        if (UIHandler.input_active && player.can_input) {
            if (Input.GetButton("Attack")) {
                player.BasicAttack();
            }
        }
        Move();
    }

    void Move() {
        if (cont.collisions.above || cont.collisions.below) {
            gravity_force.y = 0;
            velocity.y = 0;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (!player.can_input || !UIHandler.input_active) {
            input = Vector2.zero;
        }

        if (!jumping) gravity_force.y += gravity * Time.deltaTime;

        if (!player.knocked_back) {
            knocked_back_last_frame = false;
            if (UIHandler.input_active && player.can_input) {
                if (input.y < -0.25 && Input.GetButtonDown("Jump") && drop_routine == null && cont.OverPlatform()) {
                    if (cont.OverPlatform()) {
                        drop_routine = StartCoroutine(DropRoutine());
                    }
                } else if (Input.GetButtonDown("Drop") && drop_routine == null) {
                    drop_routine = StartCoroutine(DropRoutine());
                } else if (Input.GetButtonDown("Jump") && cont.collisions.below) {
                    gravity_force.y = 0;
                    velocity.y = jump_velocity;
                    StartCoroutine(JumpRoutine());
                } else if (jumping) {
                    velocity.y = jump_velocity;
                }
            } else if (jumping) {
                velocity.y = jump_velocity;
            }

            float target_velocity_x = input.x * player.speed;
            //velocity.x = Mathf.SmoothDamp(velocity.x, target_velocity_x, ref x_smooth, cont.collisions.below ? acceleration_grounded : acceleration_airborne);
            velocity.x = target_velocity_x;

            if (input.x != 0 && (cont.collisions.below || cont.collisions.below_last_frame)) {
                player.animator.SetBool("Running", true);
                if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRun")) {
                    player.animator.speed = Mathf.Abs(velocity.x / 5f);
                } else {
                    player.animator.speed = 1f;
                }
            } else {
                player.animator.SetBool("Running", false);
                player.animator.speed = 1f;
            }

            if (player.can_change_facing) {
                if (input.x < 0) {
                    flip_object.transform.localRotation = Quaternion.Euler(0,180f,0);
                } else if (input.x > 0) {
                    flip_object.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }

            cont.Move((velocity + gravity_force) * Time.deltaTime);
        } else {
            if (knocked_back_last_frame == false) gravity_force = Vector3.zero;
            knocked_back_last_frame = true;
            velocity.y = 0;
            cont.Move((gravity_force + player.knockback_force) * Time.deltaTime);
        }
        if (player.knocked_back && (cont.collisions.left || cont.collisions.right)) {
            player.CancelXKnockBack();
        }
        if (player.knocked_back && cont.collisions.above) {
            player.CancelYKnockBack();
        }
        if (player.knocked_back && cont.collisions.below) {
            player.CancelKnockBack();
        }
    }

    IEnumerator DropRoutine() {
        cont.RemovePlatformFromMask();
        float delay = .15f;
        while (delay > 0) {
            yield return null;
            delay -= Time.deltaTime;
        }
        while (Input.GetButton("Drop")) {
            yield return null;
        }
        cont.AddPlatformToMask();
        drop_routine = null;
    }

    IEnumerator JumpRoutine() {
        jumping = true;
        float time_left = max_jump_hold;
        bool held = true;
        while (time_left > 0 && held && !cont.collisions.above) {
            time_left -= Time.deltaTime;
            held = held && Input.GetButton("Jump");
            yield return null;
        }
        jumping = false;
    }
}
