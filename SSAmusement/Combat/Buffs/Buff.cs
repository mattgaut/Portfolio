using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { stat, attack, healing, on_hit, tick }

public abstract class Buff : MonoBehaviour {
    public abstract BuffType type { get; }

    [SerializeField] Sprite _icon;
    public Sprite icon {
        get { return _icon; }
        set { _icon = value; }
    }

    [SerializeField] bool timed_buff;
    [SerializeField] float remove_after;
    [SerializeField] Color _color;

    public Color color {
        get { return _color; }
    }
    public float buff_length {
        get { return remove_after; }
    }

    public void ApplyTo(ICombatant stat_entity, bool log = false) {
        Apply(stat_entity);
        if (timed_buff) {
            if (log) stat_entity.LogBuff(this);
            stat_entity.StartCoroutine(RemoveAfter(stat_entity, remove_after));
        }
    }
    protected abstract void Apply(ICombatant stat_entity);

    public void RemoveFrom(ICombatant stat_entity) {
        Remove(stat_entity);
    }
    protected abstract void Remove(ICombatant stat_entity);

    IEnumerator RemoveAfter(ICombatant remove_from, float after) {
        float time_left = remove_after;
        while (time_left > 0) {
            time_left -= Time.deltaTime;
            yield return null;
        }
        RemoveFrom(remove_from);
    }
}