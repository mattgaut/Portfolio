using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {

    public virtual AbilityType type {
       get { return AbilityType.none; }
    }

    public Card source {
        get; protected set;
    }

    public virtual void SetSource(Card c) {
        source = c;
    }

}
