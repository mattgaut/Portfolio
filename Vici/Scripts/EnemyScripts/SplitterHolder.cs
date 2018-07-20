using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterHolder : Enemy {

    [SerializeField]
    GameObject spawn;

    protected override void Awake() {
        base.Awake();
        Splitter split_1 = Instantiate(spawn, transform.position, Quaternion.identity).GetComponent<Splitter>();
        split_1.transform.SetParent(transform, true);

        sethealth = split_1.GetStartingHealth() * split_1.level;
        split_1.SetSpawn(spawn);
    }

    public override float TakeDamage(float dmg) {
        return base.TakeDamage(dmg);
    }

    public bool CheckSpawnObjects() {
        return false;
    }
}
