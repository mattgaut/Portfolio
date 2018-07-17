using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {
    public enum Position { TopRight, TopLeft, BottomRight, BottomLeft }


    static FPSCounter instance;
    Text text;
    RectTransform text_transform;
    float timer = 0, count = 0;
    public static Position position {
        get; private set;
    }
    public static bool is_on {
        get; private set;
    }

	// Use this for initialization
	void Start () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject.transform.root);
        } else {
            Destroy(gameObject);
        }
        text = GetComponent<Text>();
        text_transform = text.GetComponent<RectTransform>();
        SetPosition(position);
	}

	// Update is called once per frame
	void Update () {
        timer += Time.unscaledDeltaTime;
        count++;
        if (timer >= 1) {
            timer -= 1;
            text.text = count + "";
            count = 0;
        }
	}

    public static void SetOn(bool on) {
        if (instance) {
            instance.text.enabled = on;
            is_on = on;
        }
    }

    //
    public static void SetPosition(Position pos) {
        if (instance) {
            position = pos;
            if (pos == Position.TopLeft) {
                instance.text_transform.anchorMax = instance.text_transform.anchorMin = new Vector2(0,1);
                instance.text.alignment = TextAnchor.UpperLeft;
            }
            if (pos == Position.TopRight) {
                instance.text_transform.anchorMax = instance.text_transform.anchorMin = new Vector2(1, 1);
                instance.text.alignment = TextAnchor.UpperRight;
            }
            if (pos == Position.BottomRight) {
                instance.text_transform.anchorMax = instance.text_transform.anchorMin = new Vector2(1, 0);
                instance.text.alignment = TextAnchor.LowerRight;
            }
            if (pos == Position.BottomLeft) {
                instance.text_transform.anchorMax = instance.text_transform.anchorMin = new Vector2(0, 0);
                instance.text.alignment = TextAnchor.LowerLeft;
            }
        }
    }
}
