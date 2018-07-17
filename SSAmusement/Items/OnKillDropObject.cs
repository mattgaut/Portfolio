using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKillDropObject : OnKillItem {

    [SerializeField] List<GameObject> to_drop;

    [SerializeField][Range(0,1)]float chance;

    protected override void OnKill(Player p, ICombatant killed) {
        if (chance <= Random.Range(0f, 1f)) return;

        GameObject dropped = to_drop[Random.Range(0, to_drop.Count)];
        dropped = Instantiate(dropped);
        dropped.transform.position = killed.gameObject.transform.position + Vector3.up * 0.5f;
    }
}
