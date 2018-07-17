using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sabotage : Item {

    [SerializeField]
    float length;

    [Range(0,1)][SerializeField]
    float multiplier;

    public override void OnUse(Player user) {
        user.StartCoroutine(SabotageRoutine(user));
    }

    IEnumerator SabotageRoutine(Player user) {
        float time = length;
        foreach (Player p in GameManager.instance.GetPlayers()) {

            if (p != user) {
                p.attached_ui.item_timer.SpawnTimer(sprite, Color.red, length);
                p.car.MaxSpeed *= multiplier;
            }
        }
        while (time > 0) {
            yield return null;
            time -= Time.deltaTime;
        }
        foreach (Player p in GameManager.instance.GetPlayers()) {
            if (p != user) {
                p.car.MaxSpeed /= multiplier;
            }
        }
    }

}
