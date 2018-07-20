using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    PlayerCharacter character;
    [SerializeField]
    Rigidbody2D body;

    Vector3 movement;

    public bool player_has_control;

    [SerializeField]
    LayerMask clear_velocity;

    // Update is called once per frame
    void Update () {
        if (player_has_control && !character.dead && !UIController.instance.paused) {
            Move();
            BasicAttack();
            Ability1();
            Ability2();
            Ability3();
            Ability4();
        }
        //if (Input.GetMouseButtonDown(1)) {
        //    character.TakeDamage(100);
        //}
        if (Input.GetButtonDown("Escape")) {
            UIController.instance.TogglePause(character);
        }
    }
    void LateUpdate() {
        if (player_has_control && !character.dead && !UIController.instance.paused) {
             Turn();
        }
    }

    void Move() {
        body.velocity = Vector2.zero;

        movement.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        if (movement != Vector3.zero)
            body.MovePosition(transform.position + (movement.normalized * character.speed.value * Time.deltaTime));
    }
    void Turn() {
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //if (stats.turn_speed < 0) {         
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousepos - transform.position);
        //} else {
        //    Quaternion look_to = Quaternion.LookRotation(Vector3.forward, mousepos - transform.position);
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, look_to, stats.turn_speed * Time.deltaTime);
        //}

    }
    protected void BasicAttack() {
        if (Input.GetButtonDown("BasicAttack")) {
            character.TryUseBasicAttack();
        }
    }

    protected void Ability1() {
        if (Input.GetButtonDown("Ability1")) {
            character.TryUseAbility1();
        }
    }
    protected void Ability2() {
        if (Input.GetButtonDown("Ability2")) {
            character.TryUseAbility2();
        }
    }
    protected void Ability3() {
        if (Input.GetButtonDown("Ability3")) {
            character.TryUseAbility3();
        }
    }
    protected void Ability4() {
        if (Input.GetButtonDown("Ability4")) {
            character.TryUseAbility4();
        }
    }

    protected void OnCollisionExit(Collision coll) {
        if ((1 << coll.gameObject.layer & clear_velocity.value) != 0) {
            body.velocity = Vector3.zero;
        }
    }
}
