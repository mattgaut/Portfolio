using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {

    [SerializeField] float speed, jump_strength, hold_time;
    [SerializeField] Collider2D platform_collider;
    [SerializeField] LayerMask ground;
    Rigidbody2D body;
    Player player;
    Coroutine jump_routine, fall_routine;
    bool can_jump, holding_jump;
    int floor_count;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        floor_count = 0;
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update () {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space)) {
            player.BasicAttack();
        }
    }

    void HandleMovement() {
        Vector2 movement = new Vector2(0, body.velocity.y);
        if (Input.GetKey(KeyCode.A)) {
            movement.x -= speed;
        } else if (Input.GetKey(KeyCode.D)) {
            movement.x += speed;
        } else {
            movement.x = 0;
        }
        if (can_jump && Input.GetKeyDown(KeyCode.W) && body.velocity.y == 0) {
            if (jump_routine != null) StopCoroutine(jump_routine);
            jump_routine = StartCoroutine(Jumping());
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            if (fall_routine != null) StopCoroutine(fall_routine);
            fall_routine = StartCoroutine(FallThrough());
        }
        if (holding_jump) {
            movement.y = jump_strength;
        }

        body.velocity = movement;
    }

    IEnumerator Jumping() {
        float time = hold_time;
        can_jump = false;
        holding_jump = true;
        while (time > 0 && Input.GetKey(KeyCode.W)) {
            time -= Time.deltaTime;
            yield return null;
        }
        holding_jump = false;
    }
    IEnumerator FallThrough() {
        float time = .1f;
        platform_collider.enabled = false;
        while (time > 0 || Input.GetKey(KeyCode.S)) {
            time -= Time.deltaTime;
            yield return null;
        }
        platform_collider.enabled = true;
    }

    //private void OnCollisionEnter2D(Collision2D collision) {
    //    Debug.Log(collision.gameObject.layer + " : " + LayerMask.NameToLayer("Platform"));
    //    if (((1 << collision.gameObject.layer) & (LayerMask.NameToLayer("Platform"))) != 0) {
    //        can_jump = true;
    //        floor_count++;
    //    }
    //}
    //private void OnCollisionExit2D(Collision2D collision) {
    //    if (((1 << collision.gameObject.layer) & (LayerMask.NameToLayer("Platform"))) != 0) {
    //        floor_count--;
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & ground) != 0) {
            can_jump = true;
            floor_count++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & ground) != 0) {
            floor_count--;
            if (floor_count == 0) {
                can_jump = false;
            }
        }
    }
}
