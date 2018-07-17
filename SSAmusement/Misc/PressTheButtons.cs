using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressTheButtons : MonoBehaviour {

    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(PressButtons());
	}
	
	IEnumerator PressButtons() {
        List<string> buttons = new List<string>() { "Red", "Yellow", "Green", "Blue" };
        while (true) {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("ByrdmanIdle")) {
                float rand = Random.Range(0f, 1f);
                if (rand < .10f) {
                    float timer = 0;
                    while (timer < 2f) {
                        timer += Time.deltaTime;
                        yield return null;
                    }
                    anim.SetBool("Rest", true);
                    timer = 0;
                    while (timer < 2f) {
                        timer += Time.deltaTime;
                        yield return null;
                    }
                    anim.SetBool("Rest", false);
                } else {
                    buttons.Shuffle();
                    anim.SetTrigger(buttons[0]);
                }
                yield return null;
            }
            yield return null;
        }
    }
}
