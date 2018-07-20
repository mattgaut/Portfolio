using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fps : MonoBehaviour {

    Text text;
    float timer;
    float count;
	// Use this for initialization
	void Start () {
        timer = 0;
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.unscaledDeltaTime;
        count++;
        if (timer > 1) {
            timer = 0;
            text.text = "" + count;
            count = 0;
        }
	}
}
