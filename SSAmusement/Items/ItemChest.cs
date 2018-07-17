using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemChest : MonoBehaviour {

    Item to_spawn;
    bool opened, inside;
    Collider2D hitbox;
    [SerializeField] Sprite open_sprite;

    void Open() {
        Item new_item = Instantiate(to_spawn);
        new_item.transform.position = transform.position;
        opened = true;
        hitbox.enabled = false;
        GetComponent<SpriteRenderer>().sprite = open_sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            inside = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            inside = false;
        }
    }

    private void Awake() {
        opened = inside = false;
        hitbox = GetComponent<Collider2D>();
    }

    private void Update() {
        if (inside && Input.GetButtonDown("Interact")) {
            Open();
        }
    }

    public void SetSpawnItem(Item i) {
        to_spawn = i;
    }
}
