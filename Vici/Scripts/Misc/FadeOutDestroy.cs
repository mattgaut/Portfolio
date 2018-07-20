using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutDestroy : MonoBehaviour {

    [SerializeField]
    SpriteRenderer to_fade;

    public Coroutine StartFade(float timer) {
        return StartCoroutine(Fade(timer));
    }

    IEnumerator Fade(float timer) {
        yield return null;

        float counter = 0;
        while (counter < timer) {
            counter += Time.deltaTime;
            to_fade.color = new Color(to_fade.color.r,to_fade.color.g,to_fade.color.b, 1 - (counter / timer)); 
            yield return null;
        }

        Destroy(gameObject);
    }
}
