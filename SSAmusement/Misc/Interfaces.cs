using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    GameObject gameObject { get; }
    CapStat health {
        get;
    }
    float TakeDamage(float damage, ICombatant source);
    void TakeKnockback(ICombatant source, Vector3 force, float length = 0.5f);

    Vector3 knockback_force { get; }
    bool knocked_back { get; }
    bool invincible { get; }
}

public interface IStats : IDamageable {
    Stat power { get; }
    Stat armor { get; }
    Stat speed { get; }
    CapStat energy { get; }
}

public interface ICombatant: IDamageable, IStats {
    float DealDamage(float damage, IDamageable target, bool trigger_on_hit);
    bool alive {
        get;
    }
    void KillCredit(ICombatant killed);
    void LogBuff(Buff b);
    Coroutine StartCoroutine(IEnumerator start);
}
