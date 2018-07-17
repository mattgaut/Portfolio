using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Inventory : MonoBehaviour {

    Player player;
    List<Item> items_in_inventory;
    public int curreny {
        get; private set;
    }
    public int boss_keycards {
        get; private set;
    }
    public ReadOnlyCollection<Item> items {
        get { return new ReadOnlyCollection<Item>(items_in_inventory); }
    }

    private void Awake() {
        boss_keycards += 1;
        player = GetComponent<Player>();
        items_in_inventory = new List<Item>();
    }

    public void AddItem(Item i) {
        items_in_inventory.Add(i);
        i.transform.SetParent(transform);
        i.Pickup(player);
        UIHandler.DisplayItem(i);
    }

    public void AddKeycard(int i = 1) {
        if (i > 0) {
            boss_keycards += i;
        }
    }

    void RemoveKeycard(int i = 1) {
        if (i > 0 && boss_keycards > i) {
            boss_keycards -= i;
        }
    }

    public bool TrySpendKeycard() {
        if (boss_keycards >= 1) {
            RemoveKeycard();
            return true;
        }
        return false;
    }
}
