using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : SpellCombatant {

    protected override void ContextClickHandler() {
        if (GameManager.instance.attacking && zone == Zone.field) {
            if (GameManager.instance.current_turns_player != controller) {
                GameManager.instance.TryChooseAttackerTarget(this);
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

    public override void EOTCleanup() {
        base.EOTCleanup();
    }
}
