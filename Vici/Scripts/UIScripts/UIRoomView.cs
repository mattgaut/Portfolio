using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomView : MonoBehaviour {

    [SerializeField]
    Image image;
    [SerializeField]
    Sprite room_sprite;

    bool entered;

    public void SetFocus(bool has_focus) {
        if (has_focus) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        } else {
            image.color = new Color(image.color.r,image.color.g,image.color.b, .5f);
        }
    }
}
