using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBuff : MonoBehaviour {

    [SerializeField] Image icon_image, slider_image;
    [SerializeField] Slider slider;


    public float remaining_time { get; private set; }
    float total_time;

    private void Update() {
        remaining_time -= Time.deltaTime;
        slider.value = remaining_time / total_time;
        if (remaining_time <= 0) {
            Destroy(gameObject);
        }
    }

    public void SetBarColor(Color color) {
        slider_image.color = color;
    }

    public void SetIcon(Sprite sprite) {
        icon_image.sprite = sprite;
    }

    public void SetTime(float time) {
        total_time = time;
        remaining_time = total_time;
    }
}
