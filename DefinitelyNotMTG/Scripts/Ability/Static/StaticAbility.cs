using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class StaticAbility : Ability {

    public override AbilityType type {
        get { return AbilityType.static_; }
    }

    [SerializeField]
    List<Zone> _subscribes_to;
    [SerializeField]
    bool yours, thiers, another;
    [SerializeField]
    List<MajorCardType> affects;

    HashSet<Card> affected;

    public virtual void Awake() {
        affected = new HashSet<Card>();
    }

    public ReadOnlyCollection<Zone> subscribes_to {
        get { return new ReadOnlyCollection<Zone>(_subscribes_to); }
    }

    public virtual bool Affects(Card c) {
        if (!yours && c.controller == source.controller) {
            return false;
        }
        if (!thiers && c.controller != source.controller) {
            return false;
        }
        if (another && c == source) {
            return false;
        }
        return affects.Contains(c.major_card_type);
    }

    public void Add(Card c) {
        if (!affected.Contains(c)) {
            affected.Add(c);
            Apply(c);
        }
    }
    public void Remove(Card c) {
        if (affected.Contains(c)) {
            affected.Remove(c);
            UnApply(c);
        }
    }
    public void Clear() {
        foreach (Card c in affected) {
            UnApply(c);
        }
        affected.Clear();
    }

    protected abstract void Apply(Card c);
    protected abstract void UnApply(Card c);
}
