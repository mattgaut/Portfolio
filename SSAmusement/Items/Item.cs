using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Item : MonoBehaviour {

    [SerializeField] string _item_name;
    [SerializeField][TextArea(1, 3)] string _item_description;
    [SerializeField] SpriteRenderer sr;
    Collider2D hitbox;
    bool can_pickup, inside;

    public string item_name {
        get { return _item_name; }
    }
    public string item_description {
        get { return _item_description; }
    }
    public Sprite icon {
        get; private set;
    }
    public void Awake() {
        hitbox = GetComponent<Collider2D>();
        icon = sr.sprite;
        StartCoroutine(TurnOffWhileTouching());
    }

    protected Player owner {
        get; private set;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            player.inventory.AddItem(this);
            if (can_pickup) {
                //Player player = collision.gameObject.GetComponentInParent<Player>();
                //player.inventory.AddItem(this);
            } else {
                inside = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        inside = false;
    }

    void SetOwner(Player p) {
        owner = p;
    }

    public void Pickup(Player p) {
        SetOwner(p);
        OnPickup();
        sr.enabled = false;
        hitbox.enabled = false;
    }

    protected abstract void OnPickup();

    public void Drop(Player p) {
        if (p == owner) {
            OnDrop();
            sr.enabled = true;
            transform.parent.SetParent(null);
            hitbox.enabled = true;
            SetOwner(null);
        }
    }

    protected abstract void OnDrop();

    IEnumerator TurnOffWhileTouching() {        
        can_pickup = false;
        yield return null;
        do {
            yield return null;
        } while (inside);
        can_pickup = true;
    }
}
