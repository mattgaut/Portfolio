using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    [SerializeField] ItemChest item_chest;

    public Item SpawnItem(Item i) {
        Item new_item = Instantiate(i, transform.parent.transform);
        new_item.transform.position = transform.position;
        Destroy(gameObject);
        return new_item;
    }

    public ItemChest SpawnItemChest() {
        ItemChest new_item_chest = Instantiate(item_chest, transform.parent.transform);
        new_item_chest.transform.position = transform.position;
        Destroy(gameObject);
        return new_item_chest;
    }
}
