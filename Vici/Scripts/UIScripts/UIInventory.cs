using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {

    [SerializeField]
    Text key_text;
    [SerializeField]
    Image boss_key_image;

    public void UpdateView(Inventory inventory) {
        key_text.text = "X " + inventory.keys;
        if (inventory.boss_keys > 0) {
            boss_key_image.color = Color.white;
        } else {
            boss_key_image.color = Color.black;
        }
    }
}
