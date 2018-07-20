using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnAwakeFadeIn : MonoBehaviour {

    [SerializeField]
    Image fade_in_panel;
    [SerializeField]
    Text fade_in_text;
    [SerializeField]
    Text button_fade_in;

    void OnEnable() {
        StartCoroutine(FadeIn(2f));
    }

    IEnumerator FadeIn(float time) {
        float start_time = time;
        fade_in_panel.color = new Color(fade_in_panel.color.r, fade_in_panel.color.g, fade_in_panel.color.b, 1);
        fade_in_text.color = new Color(fade_in_text.color.r, fade_in_text.color.g, fade_in_text.color.b, 1);
        button_fade_in.color = new Color(button_fade_in.color.r, button_fade_in.color.g, button_fade_in.color.b, 1);
        while (time > 0) {
            time -= Time.deltaTime;
            fade_in_panel.color = new Color(fade_in_panel.color.r, fade_in_panel.color.g, fade_in_panel.color.b, 1f - time/start_time);
            fade_in_text.color = new Color(fade_in_text.color.r, fade_in_text.color.g, fade_in_text.color.b, 1f - time/ start_time);
            button_fade_in.color = new Color(button_fade_in.color.r, button_fade_in.color.g, button_fade_in.color.b, 1f - time/start_time);
            yield return null;
        }
        fade_in_panel.color = new Color(fade_in_panel.color.r, fade_in_panel.color.g, fade_in_panel.color.b, 1);
        fade_in_text.color = new Color(fade_in_text.color.r, fade_in_text.color.g, fade_in_text.color.b, 1);
        button_fade_in.color = new Color(button_fade_in.color.r, button_fade_in.color.g, button_fade_in.color.b, 1);
    }
}
