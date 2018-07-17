using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour {

    [SerializeField]
    Sprite _sprite;

    public Sprite sprite {
        get { return _sprite; }
    }

    public void UseItem(Player user) {
        OnUse(user);
        user.ClearItem();
    }

    public abstract void OnUse(Player user);
}
