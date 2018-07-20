using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalTriggeredAbility : TriggeredAbility {
    protected abstract bool CheckTrigger(TriggerInfo info);

    public bool TriggersFrom(TriggerInfo info) {
        return CheckTrigger(info);
    }
}