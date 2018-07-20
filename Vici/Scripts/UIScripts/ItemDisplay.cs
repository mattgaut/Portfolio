using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour {

    [SerializeField]
    Text item_name, description;

    public void DisplayItem(Item item, float length = 3f) {
        StartCoroutine(DisplayItemRoutine(item, length));
    }
    IEnumerator DisplayItemRoutine(Item item, float length) {
        item_name.enabled = true;
        description.enabled = true;

        item_name.text = item.item_name;
        description.text = item.description;
        item_name.color += new Color(0, 0, 0, 1);
        description.color += new Color(0, 0, 0, 1);
        while (length > 0f) {
            length -= Time.deltaTime;

            yield return null;
        }

        // Fade out
        float fade_out_time = 0.25f;
        length = fade_out_time;

        Color temp;
        while (length > 0) {
            length -= Time.deltaTime;

            temp = item_name.color;
            item_name.color = new Color(temp.r, temp.g, temp.b, length / fade_out_time);
            temp = description.color;
            description.color = new Color(temp.r, temp.g, temp.b, length / fade_out_time);

            yield return null;
        }

        item_name.enabled = false;
        description.enabled = false;
    }
}
