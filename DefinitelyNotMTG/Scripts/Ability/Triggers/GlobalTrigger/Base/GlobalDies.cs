using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalDies : GlobalTriggeredAbility {

    [SerializeField]
    MajorCardType triggers_from;
    [SerializeField]
    bool yours, theirs, self;

    protected override bool CheckTrigger(TriggerInfo info) {
        DiesTriggerInfo etbinfo = info as DiesTriggerInfo;
        if (etbinfo != null) {
            if (etbinfo.died.controller == source.controller && !yours) {
                return false;
            }
            if (etbinfo.died.controller != source.controller && !theirs) {
                return false;
            }
            if (ReferenceEquals(etbinfo.died, source) && !self) {
                return false;
            }
            return etbinfo.died.major_card_type == triggers_from;
        }
        return false;
    }
}
