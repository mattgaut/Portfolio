using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OpeningCrawl : MonoBehaviour {

    [SerializeField] Image fade, background;
    [SerializeField] Text text;
    [SerializeField] float fade_time, hold_time;

    [SerializeField] UnityEvent on_finish;

    public void StartCrawl() {
        StartCoroutine(Crawl());
    }

    IEnumerator Crawl() {
        // Fade in
        fade.enabled = true;
        text.enabled = true;
        background.enabled = true;
        background.color = Color.black;
        float timer = fade_time;
        while (timer > 0) {
            timer -= Time.deltaTime;
            fade.color = new Color(0, 0, 0, timer / fade_time);
            yield return null;
        }
        fade.color = Color.clear;
        // Hold
        timer = 0;
        while (timer < hold_time) {
            timer += Time.deltaTime;
            yield return null;
        }
        // Fade Out
        timer = 0;
        while (timer < fade_time) {
            timer += Time.deltaTime;
            fade.color = new Color(0, 0, 0, timer / fade_time);
            yield return null;
        }
        fade.color = Color.black;

        on_finish.Invoke();
    }
}
