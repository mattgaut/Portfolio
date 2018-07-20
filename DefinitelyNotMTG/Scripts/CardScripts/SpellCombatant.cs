using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellCombatant : SpellPermanent, ICombatant {
    [SerializeField]
    protected int _attack, _toughness;

    bool _dead = false;
    public bool dead {
        get { return damage_taken >= toughness || toughness <= 0 || _dead; }
    }

    public int toughness {
        get { return _toughness + effects.toughness; }
    }
    public int attack_power {
        get { return _attack + effects.attack_power; }
    }
    public int damage_taken {
        get; protected set;
    }
    public int health_remaining {
        get { return toughness - damage_taken; }
    }
    public bool first_strike {
        get { return triggers.keywords.first_strike; }
    }

    public void ClearDamage() {
        damage_taken = 0;
    }

    public int TakeDamage(Card source, int damage) {
        damage_taken += damage;
        if (source.triggers.keywords.deathtouch) {
            _dead = true;
        }
        return damage;
    }
    public int DealCombatDamage(IDamageable com) {
        if (attack_power > 0)
            return DealDamage(com, attack_power);
        else
            return 0;
    }

    public bool CanBeAttacked(IAttacker attacker) {
        return triggers.CanBeAttacked(attacker);
    }

}
