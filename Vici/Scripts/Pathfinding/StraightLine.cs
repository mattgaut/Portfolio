using UnityEngine;
using System.Collections;
using System;

public class StraightLine : ToGameObject {

    public override void Analyze() {
        if (target != null) {
            path[0] = target.transform.position;
        }

    }

    public override void Initiate() {
        path = new System.Collections.Generic.List<Vector3>();
        path.Add(Vector3.zero);
    }
}
