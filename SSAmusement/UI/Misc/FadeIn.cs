using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour {

    [SerializeField] Image fade;
    [SerializeField] float fade_in_length;

    Color color;
    float starting_alpha;
	// Use this for initialization
	void Start () {
        color = fade.color;
        starting_alpha = color.a;
        StartCoroutine(Fade());
	}
	

    public IEnumerator Fade() {
        while (color.a >= 0) {
            yield return null;
            color.a -= (Time.deltaTime / fade_in_length) * starting_alpha;
            fade.color = color;
        }
        fade.enabled = false;
        color.a = starting_alpha;
        fade.color = color;
        Destroy(this);
    }
}
