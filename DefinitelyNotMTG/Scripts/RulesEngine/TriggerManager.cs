using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager {

    Dictionary<TriggerType, List<TriggeredAbility>> triggers;

    public TriggerManager() {
        triggers = new Dictionary<TriggerType, List<TriggeredAbility>>();
        foreach (TriggerType t in System.Enum.GetValues(typeof(TriggerType))) {
            triggers.Add(t, new List<TriggeredAbility>());
        }
    }

    public void Subscribe(TriggerType type, GlobalTriggeredAbility ability) {
        triggers[type].Add(ability);
    }

    public void UnSubscribe(TriggerType type, GlobalTriggeredAbility ability) {
        triggers[type].Remove(ability);
    }

    public IEnumerator Trigger(TriggerInfo info) {
        foreach (GlobalTriggeredAbility ta in triggers[info.type]) {
            if (ta.TriggersFrom(info)) {
                yield return ta.OnTrigger();
            }
        }
    }
}

public class TriggerInfo {

    public TriggerType type {
        get; private set;
    }
    
    public TriggerInfo(TriggerType type) {
        this.type = type;
    }

}
public class ETBTriggerInfo : TriggerInfo {

    public Card entered {
        get; private set;
    }
    public ETBTriggerInfo(Card entered) : base(TriggerType.etb) {
        this.entered = entered;
    }

}

public class DiesTriggerInfo : TriggerInfo {

    public Card died {
        get; private set;
    }
    public DiesTriggerInfo(Card died) : base(TriggerType.dies) {
        this.died = died;
    }

}

public class DrawTriggerInfo : TriggerInfo {

    public Card drawn {
        get; private set;
    }
    public int number_drawn {
        get; private set;
    }
    public DrawTriggerInfo(Card drawn, int number_drawn_this_turn) : base(TriggerType.draws) {
        this.drawn = drawn;
        number_drawn = number_drawn_this_turn;
    }

}
public class EOTTriggerInfo : TriggerInfo {

    public Player player {
        get; private set;
    }
    public EOTTriggerInfo(Player p) : base(TriggerType.eot) {
        player = p;
    }

}
