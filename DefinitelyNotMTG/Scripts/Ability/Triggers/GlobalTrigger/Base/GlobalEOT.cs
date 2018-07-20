using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEOT : GlobalTriggeredAbility {
    [SerializeField]
    bool your_eot, opponents_eot;

    protected override bool CheckTrigger(TriggerInfo info) {
        EOTTriggerInfo trigger = info as EOTTriggerInfo;
        if (trigger != null) {
            if (trigger.player == source.controller) {
                return your_eot;
            } else {
                return opponents_eot;
            }
        } else
            return false;
    }
}
