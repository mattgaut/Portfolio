using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatItem : Item {

    [SerializeField] List<StatBuff> buffs_to_apply;

    protected override void OnDrop() {
        foreach (StatBuff b in buffs_to_apply) {
            b.RemoveFrom(owner);
        }
    }

    protected override void OnPickup() {
        foreach (StatBuff b in buffs_to_apply) {
            b.ApplyTo(owner);
        }
    }
}
