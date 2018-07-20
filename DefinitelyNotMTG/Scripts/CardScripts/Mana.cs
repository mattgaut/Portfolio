using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : PlayableCard {

    [SerializeField]
    ManaType type;

    
    public override bool CanBePlayed() {

        if (GameManager.instance.current_phase != Phase.main2 && GameManager.instance.current_phase != Phase.main1) {
            return false;
        }
        if (GameManager.instance.current_turns_player != controller) {
            return false;
        }
        return true;
    }

    public override IEnumerator OnPlay() {
        controller.mana_pool.AddMax(type, 1, true);
        yield return GameManager.instance.MoveToGraveyard(this);
    }

    protected override void ClickInHand() {
        controller.PerformAction(new GameManager.TryPlayCard(this));
    }
}
