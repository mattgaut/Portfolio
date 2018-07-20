using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRoomLayout : RoomLayout {

    [SerializeField]
    GameObject item_pedastal;
    GameObject item;

    public void SetItem(GameObject g) {
        item = Instantiate(g, item_pedastal.transform, false);
    }
}
