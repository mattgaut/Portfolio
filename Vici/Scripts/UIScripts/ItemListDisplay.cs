using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListDisplay : MonoBehaviour {

    [SerializeField]
    GameObject template;
    [SerializeField]
    float offset;

    List<GameObject> current;

    void Awake() {
        current = new List<GameObject>();
    }

    public void DisplayItemList(List<Item> items) {
        if (current != null) {
            foreach (GameObject g in current) {
                Destroy(g);
            }
        }
        current.Clear();
        for (int i = 0; i < items.Count; i++) {
            GameObject new_obj = Instantiate(template);
            new_obj.transform.SetParent(transform, false);
            new_obj.GetComponent<RectTransform>().localPosition += new Vector3(offset * ((i % 4) + 1), -offset * ((i / 4) + 1));
            new_obj.GetComponent<RectTransform>().localPosition += new Vector3((i % 4) * 75, -(i / 4) * 75, 0);
            new_obj.GetComponent<Image>().sprite = items[i].sprite;
            current.Add(new_obj);
        }
    }
}
