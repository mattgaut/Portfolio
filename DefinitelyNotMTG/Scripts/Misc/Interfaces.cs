using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEffect {

    IEnumerator OnResolution();

}

public interface IStackEffect : IEffect {
    GameObject gameObject {
        get;
    }
    Sprite stack_picture {
        get;
    }
    string description {
        get;
    }

    IEnumerator OnCancel();

    IEnumerator OnLeaveStack();

    IEnumerator OnEnterStack();
}

public interface IHighlightable {
    void Highlight(bool b);
}

public interface IDamageable {
    int health_remaining { get; }
    int TakeDamage(Card source, int damage);
}
public interface IDamages {
    int DealDamage(IDamageable dam, int damage);
}

public interface ITargetable : IHighlightable {
    bool CanBeTargeted(ITargets targeter);

    GameObject gameObject {
        get;
    }

    Player controller {
        get;
    }
}

public interface ITargets {

    Player controller {
        get;
    }

    IEnumerator TargetRoutine(System.Action<bool> legel_targets_callback);

    bool CanTarget(ITargetable target);
    bool TrueCanTarget(ITargetable target);

    string TargetingDescription();

    List<ITargetable> all_targets {
        get;
    }
}

public interface ICombatant : IDamageable, IDamages, IHighlightable {

    GameObject gameObject {
        get;
    }

    bool CanBeAttacked(IAttacker attacker);
    int DealCombatDamage(IDamageable dam);

    bool first_strike {
        get;
    }
    int attack_power {
        get;
    }
}
public interface IBlocker : ICombatant {

    bool CanBlock();
    bool CanBlock(IAttacker attacker);

    SpellCombatant card {
        get;
    }
}

public interface IAttacker : ICombatant {
    SpellCombatant card {
        get;
    }

    bool CanAttack();
    bool CanAttack(ICombatant target);
    bool CanBeBlocked(IBlocker blocker);
}