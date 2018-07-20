using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManEnemyController : EnemyController {

    BossMan bossman;

    protected override void Start() {
        base.Start();
        bossman = (BossMan)character;
        StartCoroutine(AILoop());
    }

    protected override void Update() {
    }

    IEnumerator AILoop() {
        while (!bossman.dead) {
            yield return bossman.StartJumpTowardsPlayer(GameManager.instance.player, Random.Range(3, 6));
            yield return bossman.StartFire(GameManager.instance.player);
        }
    }
}
