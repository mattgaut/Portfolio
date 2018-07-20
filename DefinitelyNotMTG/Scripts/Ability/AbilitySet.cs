using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class AbilitySet : MonoBehaviour {

    [SerializeField]
    Card source;

    [SerializeField]
    List<TriggeredAbility> local_triggers;
    [SerializeField]
    List<GlobalTriggeredAbility> global_triggers;
    [SerializeField]
    List<StaticAbility> static_abilities;
    [SerializeField]
    KeywordAbilities _keywords;
    public KeywordAbilities keywords {
        get { return _keywords; }
    }
    Dictionary<TriggerType, List<TriggeredAbility>> local;
    Dictionary<TriggerType, List<GlobalTriggeredAbility>> global;

    public void Awake() {
        local = new Dictionary<TriggerType, List<TriggeredAbility>>();
        global = new Dictionary<TriggerType, List<GlobalTriggeredAbility>>();
        foreach (StaticAbility s in static_abilities) {
            s.SetSource(source);
        }
        foreach (TriggerType t in System.Enum.GetValues(typeof(TriggerType))) {
            local.Add(t, new List<TriggeredAbility>());
            global.Add(t, new List<GlobalTriggeredAbility>());
        }
        foreach (TriggeredAbility t in local_triggers) {
            local[t.trigger_type].Add(t);
            t.SetSource(source);
        }
        foreach (GlobalTriggeredAbility t in global_triggers) {
            global[t.trigger_type].Add(t);
            t.SetSource(source);
        }
    }

    public void AddTrigger(TriggerType type, TriggeredAbility ab) {
        local[type].Add(ab);
    }
    public void AddTrigger(TriggerType type, GlobalTriggeredAbility ab) {
        global[type].Add(ab);
    }
    public void RemoveTrigger(TriggerType type, TriggeredAbility ab) {
        local[type].Remove(ab);
    }
    public void RemoveTrigger(TriggerType type, GlobalTriggeredAbility ab) {
        global[type].Remove(ab);
    }

    public List<TriggeredAbility> GetLocalTriggers(TriggerType type) {
        return local[type];
    }

    public void Subscribe() {
        SubscribeStaticAbilities();
        SubscribeTriggers();
    }
    public void UnSubscribe() {
        UnSubscribeStaticAbilities();
        UnSubscribeTriggers();
    }

    public void SubscribeTriggers() {
        foreach (TriggerType t in System.Enum.GetValues(typeof(TriggerType))) {
            if (global[t].Count > 0) {
                foreach (GlobalTriggeredAbility ta in global[t]) {
                    GameManager.instance.trigger_manager.Subscribe(t, ta);
                }
            }
        }

    }
    public void UnSubscribeTriggers() {
        foreach (TriggerType t in System.Enum.GetValues(typeof(TriggerType))) {
            if (global[t].Count > 0) {
                foreach (GlobalTriggeredAbility ta in global[t]) {
                    GameManager.instance.trigger_manager.UnSubscribe(t, ta);
                }
            }
        }
    }

    public void SubscribeStaticAbilities() {
        foreach (StaticAbility s in static_abilities) {
            GameManager.instance.static_ability_manager.Subscribe(s);
        }
    }
    public void UnSubscribeStaticAbilities() {
        foreach (StaticAbility s in static_abilities) {
            GameManager.instance.static_ability_manager.UnSubscribe(s);
        }
    }
    public bool CanAttack() {
        return !keywords.defender;
    }

    public bool CanBeAttacked(IAttacker attacker) {
        if (keywords.flying && !attacker.card.triggers.keywords.flying) {
            return false;
        }
        return true;
    }
    public bool CanBeBlocked(IBlocker blocker) {
        if (!blocker.card.triggers.keywords.flying && keywords.flying) {
            return false;
        }
        if (keywords.unblockable) {
            return false;
        }
        return true;
    }
}
