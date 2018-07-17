using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkSplotch : Item {

    [SerializeField]
    float length;

    public override void OnUse(Player user) {
        user.StartCoroutine(SabotageRoutine(user));
    }

    IEnumerator SabotageRoutine(Player user) {
        float time = length;
        foreach (Player p in GameManager.instance.GetPlayers()) {

            if (p != user) {
                p.attached_ui.item_timer.SpawnTimer(sprite, Color.red, length);
                p.attached_ui.ShowSplotch(true);
            }
        }
        while (time > 0) {
            yield return null;
            foreach (Player p in GameManager.instance.GetPlayers()) {
                if (p != user) {
                    p.attached_ui.ShowSplotch(true);
                }
            }
            time -= Time.deltaTime;
        }
        foreach (Player p in GameManager.instance.GetPlayers()) {
            if (p != user) {
                p.attached_ui.ShowSplotch(false);
            }
        }
    }
}
