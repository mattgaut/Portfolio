using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff {

    public abstract void ApplyBuff();
    public abstract void RemoveBuff();

    public void ApplyTimedBuff(float time) {
        LevelManager.instance.StartCoroutine(RemoveAfter(time));
    }

    IEnumerator RemoveAfter(float time) {
        ApplyBuff();
        while (time > 0) {
            time -= Time.deltaTime;
            yield return null;
        }
        RemoveBuff();
    }
}

public class DelegateBuff : Buff{

    public delegate void OnApplyRemove(Character c);
    OnApplyRemove apply, remove;

    Character affected_character;

    public DelegateBuff(Character c, OnApplyRemove apply, OnApplyRemove remove) {
        this.apply = apply;
        this.remove = remove;
        affected_character = c;
    }

    public override void ApplyBuff() {
        apply(affected_character);
    }
    public override void RemoveBuff() {
        remove(affected_character);
    }
}
