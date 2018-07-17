using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitLifesteal : OnHitPowerUp {

    [SerializeField] float lifesteal_percent;
    protected override void OnHit(Player p, float pre_damage, float post_damage, IDamageable hit) {
        p.health.current += post_damage * lifesteal_percent;
    }
}
