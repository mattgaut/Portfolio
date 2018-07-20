using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOpponent : Spell {

    [SerializeField]
    int damage;

    public override IEnumerator OnResolution() {
        if (controller == GameManager.instance.player1) {
            GameManager.instance.player2.TakeDamage(this, damage);
        } else {
            GameManager.instance.player1.TakeDamage(this, damage);
        }
        yield return null;
    }
}
