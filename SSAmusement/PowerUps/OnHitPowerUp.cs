using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitPowerUp : PowerUp {

    protected Player player;

    [SerializeField] float _length;
    public override float length {
        get {
            return _length;
        }
    }

    protected override void BeforeDestroy() {
        Remove();
    }

    protected override void AddPowerup(Player p) {
        base.AddPowerup(p);
        p.AddOnHit(OnHit);
        player = p;    
    }

    protected void Remove() {
        player.RemoveOnHit(OnHit);
    }

    protected abstract void OnHit(Player p, float pre_damage, float post_damage, IDamageable hit);
}
