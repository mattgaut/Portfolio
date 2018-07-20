using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour {

    [SerializeField]
    protected Character character;
    [SerializeField]
    protected Rigidbody2D body;

    protected PlayerCharacter target;

    protected Vector3 movement;

    protected virtual void Start() {
        target = GameManager.instance.player;
    }

    protected virtual void Update() {
        if (!character.stunned) {
            Move();
        }
    }

    protected virtual void Move() {
        //movement = pathfinder.move_towards - transform.position;

        if (movement != Vector3.zero)
            body.MovePosition(transform.position + (movement.normalized * character.speed.value * Time.deltaTime));
        Turn();
    }
    protected virtual void Turn() { 
        transform.rotation = Quaternion.LookRotation(Vector3.forward, movement);
    }
}
