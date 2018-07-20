using UnityEngine;
using System.Collections;

public abstract class ToGameObject : PathfinderAgent {

    public override void SetTarget(GameObject target) {
        this.target = target;
    }

}
