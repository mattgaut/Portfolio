using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : Card, IStackEffect, IDamages {
    public Sprite stack_picture {
        get {
            return art;
        }
    }

    public string description {
        get {
            return card_description;
        }
    }

    public GameObject stack_object {
        get {
            return gameObject;
        }
    }

    [SerializeField]
    ManaCost _mana_cost;
    public ManaCost mana_cost {
        get { return _mana_cost; }
    }

    protected override void ClickInHand() {
        controller.PerformAction(new GameManager.TryPlaySpell(this));
    }

    public virtual IEnumerator OnCancel() {
        throw new NotImplementedException();
    }

    public abstract IEnumerator OnResolution();

    public virtual IEnumerator OnEnterStack() {
        yield return null;
    }

    public virtual IEnumerator OnLeaveStack() {
        yield return GameManager.instance.MoveToGraveyard(this);
    }
}

[Serializable]
public class ManaCost {
    [SerializeField]
    int _generic, _red, _yellow, _blue, _black, _green;

    public int generic {
        get { return _generic; }
    }
    public int red {
        get { return _red; }
    }
    public int yellow {
        get { return _yellow; }
    }
    public int blue {
        get { return _blue; }
    }
    public int black {
        get { return _black; }
    }
    public int green {
        get { return _green; }
    }
}
