using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPermanent : Spell {

    public bool exhausted {
        get; private set;
    }
    bool _summoning_sick;
    public bool summoning_sick {
        get { return _summoning_sick && !triggers.keywords.haste; } private set { _summoning_sick = value; }
    }

    protected override void Awake() {
        base.Awake();
        exhausted = false;
        summoning_sick = true;
    }

    public bool CanBeExhausted() {
        return !exhausted && !summoning_sick;
    }
    public void ExhaustUnit(bool exhausted) {
        this.exhausted = exhausted;
    }

    public override IEnumerator OnResolution() {
        yield return GameManager.instance.MoveToField(this);
    }

    public override IEnumerator OnLeaveStack() {
        yield return null;
    }

    public override void BeginTurn() {
        base.BeginTurn();
        exhausted = false;
        summoning_sick = false;
        UpdateDisplay();
    }
}
