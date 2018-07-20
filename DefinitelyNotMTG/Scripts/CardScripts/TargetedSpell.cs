using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetedSpell : Spell, ITargets {

    [SerializeField]
    int required_targets, max_targets;
    [SerializeField]
    List<Zone> legal_target_zones;
    [SerializeField]
    List<MajorCardType> can_target_types;
    [SerializeField]
    bool can_target_players;

    protected List<ITargetable> targets;
    public List<ITargetable> all_targets {
        get { return targets; }
    }
    public bool CanTarget(ITargetable target) {
        if (targets.Count == max_targets) {
            return false;
        }
        if (targets.Contains(target)) {
            return false;
        }
        return TrueCanTarget(target);
    }

    public virtual bool TrueCanTarget(ITargetable target) {
        Card c = target as Card;
        if (c != null) {
            return legal_target_zones.Contains(c.zone) && can_target_types.Contains(c.major_card_type);
        }
        Player p = target as Player;
        if (p != null) {
            return can_target_players;
        }
        return false;
    }

    public abstract string TargetingDescription();

    public virtual IEnumerator TargetRoutine(System.Action<bool> legal_targets_callback) {

        legal_targets_callback(true);
        targets = GameManager.instance.StartTargeting(this, true, required_targets, max_targets, this, TargetingDescription());

        bool cancelled = false;
        while (!(GameManager.instance.active_player.current_action as ConfirmAction != null && targets.Count >= required_targets)) {
            if (GameManager.instance.active_player.current_action as CancelAction != null) {
                cancelled = true;
                break;
            }
            yield return null;
        }
        GameManager.instance.EndTargeting();
        if (cancelled) {
            legal_targets_callback(false);
        } else {
            legal_targets_callback(targets.Count >= required_targets);
        }
    }

    public override IEnumerator OnResolution() {
        targets = GameManager.StillLegalTargets(targets, TrueCanTarget, this);
        if (targets.Count >= 1) {
            yield return ResolveOnTargets();
        }
    }

    protected virtual IEnumerator ResolveOnTargets() {
        yield return null;
    }
}
