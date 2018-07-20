using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : SpellCombatant, IAttacker, IBlocker {

    public SpellCombatant card {
        get { return this; }
    }

    protected override void ContextClickHandler() {
        if (GameManager.instance.attacking && zone == Zone.field) {
            if (GameManager.instance.current_turns_player == controller) {
                GameManager.instance.StartAttackerTargeting(this);
            } else {
                GameManager.instance.TryChooseAttackerTarget(this);
            }
        } else if (GameManager.instance.blocking && zone == Zone.field) {
            if (GameManager.instance.current_turns_player != controller) {
                GameManager.instance.StartBlockerTargeting(this);
            } else {
                GameManager.instance.TryChooseBlockerTarget(this);
            }
        } else if (GameManager.instance.ordering_blockers && zone == Zone.field) {
            if (GameManager.instance.current_turns_player != controller) {
                GameManager.instance.TryAddOrderTarget(this);
            }
        } else if (GameManager.instance.targeting && GameManager.instance.targeter.controller == GameManager.instance.player1) {
            GameManager.instance.ToggleTarget(this);
        } else if (controller == GameManager.instance.active_player) {
            if (zone == Zone.hand) {
                ClickInHand();
            } else if (zone == Zone.library) {
                ClickInLibrary();
            } else if (zone == Zone.exile) {
                ClickInExile();
            } else if (zone == Zone.field) {
                ClickInField();
            } else if (zone == Zone.graveyard) {
                ClickInGraveyard();
            }
        }
        GameManager.instance.Hover(false, this);
        GameManager.instance.Hover(true, this);
    }

    public bool CanAttack() {
        if (zone != Zone.field) {
            return false;
        }
        return CanBeExhausted() && triggers.CanAttack();
    }

    public bool CanAttack(ICombatant target) {
        if (!CanAttack()) {
            return false;
        }
        Player p = target as Player;
        if (p != null) {
            return p != controller;
        }
        Card c = target as Card;
        if (c != null) {
            return c.controller != controller;
        }
        return false;
    }

    public bool CanBlock() {
        return !exhausted;
    }

    public bool CanBlock(IAttacker attacker) {
        Player p = attacker as Player;
        if (p != null) {
            return p != controller;
        }
        Card c = attacker as Card;
        if (c != null) {
            return c.controller != controller;
        }
        return false;
    }

    public virtual bool CanBeBlocked(IBlocker blocker) {
        return triggers.CanBeBlocked(blocker);
    }

    public override void EOTCleanup() {
        base.EOTCleanup();
        ClearDamage();
    }
}
