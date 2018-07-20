using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate IEnumerator OnTargetedResolutionDelegate(List<ITargetable> targets);
public delegate IEnumerator OnResolutionDelegate();
public delegate bool CanTargetDelegate(ITargetable target);
public delegate bool TargetsPossible();
public delegate bool TriggersFromDelegate(TriggerInfo info);
