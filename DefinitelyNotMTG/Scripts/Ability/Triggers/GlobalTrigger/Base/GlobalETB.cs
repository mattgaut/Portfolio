using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalETB : GlobalTriggeredAbility {

    [SerializeField]
    MajorCardType triggers_from;
    [SerializeField]
    bool yours, theirs, self;

    protected override bool CheckTrigger(TriggerInfo info) {
        ETBTriggerInfo etbinfo = info as ETBTriggerInfo;
        if (etbinfo != null) {
            if (etbinfo.entered.controller == source.controller && !yours) {
                return false;
            }
            if (etbinfo.entered.controller != source.controller && !theirs) {
                return false;
            }
            if (ReferenceEquals(etbinfo.entered, this) && !self) {
                return false;
            }
            return etbinfo.entered.major_card_type == triggers_from;
        }
        return false;
    }
}
