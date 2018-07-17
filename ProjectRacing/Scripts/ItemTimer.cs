using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTimer : MonoBehaviour {

    [SerializeField]
    Image fill_image, sprite;

    public void Init(Sprite s, Color c) {
        fill_image.color = c;
        sprite.sprite = s;
    }

    public void SetFill(float f) {
        fill_image.fillAmount = f;
    }
}
