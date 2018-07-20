using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableCard : Card {

    protected override void ClickInHand() {
        base.ClickInHand();
    }

    public abstract IEnumerator OnPlay();

    public abstract bool CanBePlayed();
}
