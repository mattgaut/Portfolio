using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAbilityManager {

    Dictionary<Zone, List<StaticAbility>> abilities;

    public StaticAbilityManager() {
        abilities = new Dictionary<Zone, List<StaticAbility>>();
        foreach (Zone t in System.Enum.GetValues(typeof(Zone))) {
            abilities.Add(t, new List<StaticAbility>());
        }
    }

    public void Subscribe(StaticAbility ability) {
        foreach (Zone z in ability.subscribes_to) {
            abilities[z].Add(ability);
            foreach (Card c in GameManager.instance.GetCardsInZone(z)) {
                if (ability.Affects(c))
                    ability.Add(c);
            }
        }
    }

    public void UnSubscribe(StaticAbility ability) {
        foreach (Zone z in ability.subscribes_to) {
            abilities[z].Remove(ability);
        }
        ability.Clear();
    }

    public void AddToStaticAbilities(Zone z, Card c) {
        foreach (StaticAbility ta in abilities[z]) {
            if (ta.Affects(c)) {
                ta.Add(c);
            }
        }
    }
    public void RemoveFromStaticAbilities(Zone z, Card c) {
        foreach (StaticAbility ta in abilities[z]) {
            ta.Remove(c);
        }
    }
}
