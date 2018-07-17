using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TickEffectBuff : Buff {
    public override BuffType type {
        get {
            return BuffType.tick;
        }
    }

    [SerializeField] float tick_rate;
    Dictionary<ICombatant, List<Coroutine>> tick_routines;

    public void Awake() {
        tick_routines = new Dictionary<ICombatant, List<Coroutine>>();
    }

    protected override void Apply(ICombatant stat_entity) {
        if (!tick_routines.ContainsKey(stat_entity)) {
            tick_routines.Add(stat_entity, new List<Coroutine>());
        }
        tick_routines[stat_entity].Add(StartCoroutine(Tick(stat_entity)));
    }

    protected override void Remove(ICombatant stat_entity) {
        StopCoroutine(tick_routines[stat_entity][0]);
        tick_routines[stat_entity].RemoveAt(0);
    }

    IEnumerator Tick(ICombatant stat_entity) {
        float timer = 0;
        while (true) {
            timer += Time.deltaTime;
            if (timer > tick_rate) {
                timer -= tick_rate;
                if (stat_entity == null || !stat_entity.alive) break;
                TickEffect(stat_entity);
            }
            yield return null;
        }
    }

    protected abstract void TickEffect(ICombatant stat_entity);
}
