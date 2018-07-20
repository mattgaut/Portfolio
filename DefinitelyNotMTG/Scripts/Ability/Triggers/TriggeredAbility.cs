using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggeredAbility : Ability, IEffect {

    public Sprite stack_picture {
        get {
            return source.art;
        }
    }
    public override AbilityType type {
        get {
            return AbilityType.triggered;
        }
    }

    [SerializeField]
    protected TriggerType _type;
    public TriggerType trigger_type {
        get { return _type; }
    }

    [SerializeField]
    string _description;
    public virtual string description {
        get { return _description; } protected set { _description = value; }
    }

    public virtual IEnumerator OnTrigger() {
        yield return GameManager.instance.AddToStack(new TriggerInstance(this));
    }

    public virtual IEnumerator OnResolution() {
        yield return null;
    }

    public virtual IEnumerator OnCancel() {
        yield return null;
    }

    public virtual IEnumerator OnLeaveStack() {
        yield return null;
    }

    public class TriggerInstance : IStackEffect, ITargetable {
        public TriggeredAbility ability {
            get; private set;
        }
        public TriggerInstance(TriggeredAbility ability) {
            this.ability = ability;
        }

        public Player controller {
            get { return ability.source.controller; }
        }
        public string description {
            get {
                return ability.description;
            }
        }

        public GameObject gameObject {
            get; set;
        }


        public Sprite stack_picture {
            get {
                return ability.stack_picture;
            }
        }

        public IEnumerator OnCancel() {
            yield return ability.OnCancel();
        }

        public IEnumerator OnLeaveStack() {
            yield return ability.OnLeaveStack();
        }

        public IEnumerator OnEnterStack() {
            yield return null;
        }

        public IEnumerator OnResolution() {
            yield return ability.OnResolution();
        }

        public bool CanBeTargeted(ITargets targeter) {
            return false;
        }

        public void Highlight(bool highlight) {
            gameObject.GetComponent<DisplayTrigger>().Highlight(highlight);
        }
    }
}
