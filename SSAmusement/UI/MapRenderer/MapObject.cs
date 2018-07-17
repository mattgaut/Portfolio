using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class MapObject : MonoBehaviour {

    public Vector3 center { get; private set; }

    float width, height;
    RectTransform rt;
    Image image;

    [SerializeField] Sprite active_sprite, inactive_sprite;

    public void Awake() {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetToRoom(Room r, float scale) {
        rt = GetComponent<RectTransform>();
        width = r.template.Width();
        height = r.template.Height();
        center = (r.grid_position + r.template.Center()) * scale;
        transform.localPosition = center;
        rt.sizeDelta = new Vector2(width * scale, height * scale);
    }

    public void SetSprite(bool active) {
        if (active) {
            image.sprite = active_sprite;
        } else {
            image.sprite = inactive_sprite;
        }
    }
}
