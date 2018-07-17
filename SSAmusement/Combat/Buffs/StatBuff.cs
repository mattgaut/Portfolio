using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : Buff {

    [SerializeField] bool health, energy, power, armor, speed;
    [SerializeField] float _flat, _multi;

    public float flat {
        get { return _flat; }
    }
    public float multi {
        get { return _multi; }
    }

    public override BuffType type {
        get {
            return BuffType.stat;
        }
    }

    protected override void Apply(ICombatant stat_entity) {
        if (health) {
            stat_entity.health.AddBuff(this);
        }
        if (energy) {
            stat_entity.energy.AddBuff(this);
        }
        if (power) {
            stat_entity.power.AddBuff(this);
        }
        if (armor) {
            stat_entity.armor.AddBuff(this);
        }
        if (speed) {
            stat_entity.speed.AddBuff(this);
        }
    }

    protected override void Remove(ICombatant stat_entity) {
        if (health) {
            stat_entity.health.RemoveBuff(this);
        }
        if (energy) {
            stat_entity.energy.RemoveBuff(this);
        }
        if (power) {
            stat_entity.power.RemoveBuff(this);
        }
        if (armor) {
            stat_entity.armor.RemoveBuff(this);
        }
        if (speed) {
            stat_entity.speed.RemoveBuff(this);
        }
    }
}
