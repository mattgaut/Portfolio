using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalDraw : GlobalTriggeredAbility {
    [SerializeField]
    int cards_drawn_before_trigger;
    [SerializeField]
    bool repeats;
    protected override bool CheckTrigger(TriggerInfo info) {
        DrawTriggerInfo trigger = info as DrawTriggerInfo;
        if (trigger != null) {
            if (repeats ? trigger.number_drawn > cards_drawn_before_trigger : trigger.number_drawn == cards_drawn_before_trigger + 1) {
                return true;
            }
        }
        return false;
    }
}
