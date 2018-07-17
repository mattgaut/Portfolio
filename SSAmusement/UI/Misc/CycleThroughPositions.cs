using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CycleThroughPositions : MonoBehaviour {

    [SerializeField] Image fade_object;
    [SerializeField] List<Vector3> positions;
    [SerializeField] float fade_timer, time_between_scenes;

    private void Start() {
        StartCoroutine(Cycle());
    }

    IEnumerator Cycle() {
        int count = 0;
        while (true) {          
            transform.position = positions[count];
            count = (count + 1) % positions.Count;
            yield return FadeIn(fade_timer);
            yield return new WaitForSeconds(time_between_scenes);
            yield return FadeOut(fade_timer);
        }
    }

    IEnumerator FadeIn(float fade_length) {
        Color c = Color.black;
        float timer = 0;
        while (timer < fade_length) {
            timer += Time.deltaTime;
            c.a = 1 - (timer / fade_length);
            fade_object.color = c;
            yield return null;
        }
        c.a = 0;
        fade_object.color = c;
    }

    IEnumerator FadeOut(float fade_length) {
        Color c = Color.black;
        float timer = 0;
        while (timer < fade_length) {
            timer += Time.deltaTime;
            c.a = (timer / fade_length);
            fade_object.color = c;
            yield return null;
        }
        c.a = 1;
        fade_object.color = c;
    }
}
