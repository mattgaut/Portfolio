using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalTargetedTriggeredAbility : GlobalTriggeredAbility, ITargets {

    protected List<ITargetable> targets;
    public List<ITargetable> all_targets {
        get { return targets; }
    }

    [SerializeField]
    protected List<Zone> _targetable_zones;
    [SerializeField]
    protected List<MajorCardType> _targetable_types;
    [SerializeField]
    protected bool can_target_players, can_target_itself;

    [SerializeField]
    protected int _max_targets, _min_targets;
    public int max_targets {
        get { return _max_targets; } protected set { _max_targets = value; }
    }
    public int min_targets {
        get { return _min_targets; } protected set { _min_targets = value; }
    }

    public void Awake() {
        if (max_targets < min_targets) {
            max_targets = min_targets;
        }
    }

    public Player controller {
        get { return source.controller; }
    }

    public override IEnumerator OnTrigger() {
        yield return TargetRoutine();
        yield return GameManager.instance.AddToStack(new TriggerInstance(this));
    }

    public abstract string TargetingDescription();
    public virtual IEnumerator TargetRoutine(Action<bool> legel_targets_callback = null) {
        if (LegalTargetsPossible()) {
            targets = GameManager.instance.StartTargeting(this, false, min_targets, max_targets, source, TargetingDescription());
            while (!(targets.Count >= min_targets && GameManager.instance.active_player.current_action as ConfirmAction == null)) {
                yield return null;
            }
            GameManager.instance.EndTargeting();
        }
    }

    public bool CanTarget(ITargetable target) {
        return !targets.Contains(target) && targets.Count < max_targets && TrueCanTarget(target);
    }
    public virtual bool TrueCanTarget(ITargetable target) {
        if (target as Player != null) {
            return can_target_players;
        }
        if (target as TriggerInstance != null) {
            return _targetable_zones.Contains(Zone.stack);
        }
        Card c = target as Card;
        if (c != null) {
            return _targetable_zones.Contains(c.zone) && _targetable_types.Contains(c.major_card_type) && (can_target_itself ? true : c != source);
        }
        return false;
    }
    public virtual bool LegalTargetsPossible() {
        return GameManager.instance.GetPossibleTargets(this).Count >= min_targets;
    }
}