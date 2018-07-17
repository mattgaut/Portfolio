using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnKillItem : Item {

    protected override void OnDrop() {
        owner.RemoveOnKill(OnKill);
    }

    protected override void OnPickup() {
        owner.AddOnKill(OnKill);
    }

    protected abstract void OnKill(Player p, ICombatant killed);
}
