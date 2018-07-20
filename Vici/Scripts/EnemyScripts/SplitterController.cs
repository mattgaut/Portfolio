using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterController : EnemyController {

    Splitter splitter; 

    protected void Awake() {
        splitter = (Splitter)character;
    }

    protected override void Move() {
        movement = splitter.movement;
        body.MovePosition(transform.position + (splitter.movement.normalized * splitter.speed.value * Time.deltaTime));
    }
}
