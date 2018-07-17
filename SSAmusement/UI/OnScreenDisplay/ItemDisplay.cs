using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemDisplay : MonoBehaviour {

    Image image;

    public void Display(Item i) {
        if (image == null)
            image = GetComponent<Image>();
        image.sprite = i.icon;
    }
}
