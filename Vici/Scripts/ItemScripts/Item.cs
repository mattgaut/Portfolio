using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour {

    [SerializeField]
    string _item_name, _description;
    public string item_name {
        get { return _item_name; }
    }
    public string description {
        get { return _description; }
    }

    public Sprite sprite {
        get { return sprite_renderer.sprite; }
    }

    [SerializeField]
    SpriteRenderer sprite_renderer;

    PlayerCharacter applied_character;

    public void Pickup(PlayerCharacter pc) {
        GetComponent<Collider2D>().enabled = false;
        sprite_renderer.enabled = false;
        pc.inventory.AddItemToInventory(this);
        applied_character = pc;
        OnPickup(pc);
    }
    protected abstract void OnPickup(PlayerCharacter pc);

    public void Drop() {
        OnDrop(applied_character);
        applied_character = null;
    }
    protected abstract void OnDrop(PlayerCharacter pc);

    void OnTriggerEnter2D(Collider2D coll) {
        if ((1 << coll.gameObject.layer & 1 << LayerMask.NameToLayer("Player")) != 0) {
            Pickup(coll.gameObject.GetComponent<PlayerCharacter>());
        }
    }

}
