using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessage : MonoBehaviour {

    public static UIMessage instance;
    [SerializeField]
    Text text;
    [SerializeField]
    Outline outline;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    Coroutine c;
    public void DisplayMessage(string message, float time = 2.5f, float fade_time = .25f) {
        if (c != null) {
            StopCoroutine(c);
        }
        c = StartCoroutine(DisplayMessageRoutine(message, time, fade_time));
    }

    public IEnumerator DisplayMessageRoutine(string message, float time, float fade_time) {
        text.enabled = true;
        text.text = message;
        text.color = Color.red;
        outline.effectColor = Color.black;
        while (time > 0) {
            time -= Time.deltaTime;
            yield return null;
        }
        float start_fade = fade_time;
        while (fade_time > 0) {
            fade_time -= Time.deltaTime;
            text.color = new Color(1f, 0, 0, fade_time/start_fade);
            outline.effectColor = new Color(0f, 0f, 0f, fade_time / start_fade);
            yield return null;
        }
        text.enabled = false;
    }
}
