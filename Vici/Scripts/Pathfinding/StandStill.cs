using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StandStill : PathfinderAgent {

    public override Vector3 move_towards {
        get {
            return transform.position;
        }
    }


    public override void Analyze() {
        
    }

    public override void Initiate() {
    }

    public override void SetTarget(GameObject target) {
        
    }
}
