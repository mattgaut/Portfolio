using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayStats : MonoBehaviour {

    [SerializeField]
    Text strength, intelligence, dexterity, vitality;

    [SerializeField]
    ItemListDisplay item_list;

    public void DisplayStats(PlayerCharacter stats) {
        strength.text = "" + (int)stats.strength.value;
        intelligence.text = "" + (int)stats.intelligence.value;
        dexterity.text = "" + (int)stats.dexterity.value;
        vitality.text = "" + (int)stats.vitality.value;

        item_list.DisplayItemList(new List<Item>(stats.inventory.items));
    }
}
