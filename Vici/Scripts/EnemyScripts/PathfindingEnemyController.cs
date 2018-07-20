using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathfindingEnemyController : EnemyController {

    [SerializeField]
    PathfinderAgent pathfinder;

    void Awake() {
        target = FindObjectOfType<PlayerCharacter>();
    }

    protected override void Update() {
        if (!character.stunned) {
            pathfinder.Analyze();
            Move();
        }
    }

    protected override void Move() {
        movement = pathfinder.move_towards - transform.position;

        if (movement != Vector3.zero)
            body.MovePosition(transform.position + (movement.normalized * character.speed.value * Time.deltaTime));
        Turn();
    }
}