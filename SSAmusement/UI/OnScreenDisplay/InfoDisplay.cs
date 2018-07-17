using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour {

    [SerializeField] Text main_text, sub_text;
    [SerializeField] Image image;
    [SerializeField] float fade_out_length, fade_in_length;

    List<DisplayInfo> display_queue;
    bool busy;
    Coroutine coroutine;

    private void Awake() {
        display_queue = new List<DisplayInfo>();
        busy = false;
    }

    public void Display(string main_text, float length, Color color, bool interrupt = false) {
        DisplayInfo di = new DisplayInfo(main_text, color, length);
        PushQueue(di, interrupt);
    }
    public void Display(string main_text, string sub_text, float length, Color color, bool interrupt = false) {
        DisplayInfo di = new DisplayInfo(main_text, sub_text, color, length);
        PushQueue(di, interrupt);
    }
    public void Display(string main_text, Sprite sprite, float length, Color color, bool interrupt = false) {
        DisplayInfo di = new DisplayInfo(main_text, sprite, color, length);
        PushQueue(di, interrupt);
    }
    public void Display(string main_text, string sub_text, Sprite sprite, float length, Color color, bool interrupt = false) {
        DisplayInfo di = new DisplayInfo(main_text, sub_text, sprite, color, length);
        PushQueue(di, interrupt);
    }

    void PushQueue(DisplayInfo di, bool interrupt) {
        if (!busy) {
            display_queue.Add(di);
            StartDisplay(di);
        } else if (interrupt) {
            display_queue.Insert(1, di);
            StopCoroutine(coroutine);
            PopQueue();
        } else {
            display_queue.Add(di);
        }
    }

    void PopQueue() {
        display_queue.RemoveAt(0);
        if (display_queue.Count > 0) {
            StartDisplay(display_queue[0]);
        }
    }

    void StartDisplay(DisplayInfo di) {
        busy = true;
        coroutine = StartCoroutine(Display(di));
    }

    IEnumerator Display(DisplayInfo di) {
        float time = fade_in_length;
        main_text.enabled = true;
        main_text.color = di.color - new Color(0, 0, 0, 1);
        main_text.text = di.main_text;
        if (sub_text != null) {
            sub_text.enabled = true;
            sub_text.color = di.color - new Color(0, 0, 0, 1);
            sub_text.text = di.sub_text;
        }
        if (image != null) {
            image.enabled = true;
            image.sprite = di.sprite;
            image.color = Color.white;
        }


        while (time > 0) {
            time -= Time.deltaTime;
            main_text.color = di.color - new Color(0, 0, 0, time / fade_in_length);
            if (sub_text != null) sub_text.color = di.color - new Color(0, 0, 0, time / fade_in_length);
            if (image != null) image.color = (Color.white - Color.black * time / fade_in_length);
            yield return null;
        }
        time = di.length;
        while (time > 0) {
            time -= Time.deltaTime;
            yield return null;
        }
        time = fade_out_length;
        while (time > 0) {
            time -= Time.deltaTime;
            main_text.color = di.color - new Color(0, 0, 0, 1 - time / fade_out_length);
            if (sub_text != null) sub_text.color = di.color - new Color(0, 0, 0, 1 - time / fade_in_length);
            if (image != null) image.color = (Color.white - Color.black * (1 - time / fade_in_length));
            yield return null;
        }
        main_text.enabled = false;
        sub_text.enabled = false;
        image.enabled = false;
        busy = false;
        PopQueue();
    }

    struct DisplayInfo {
        public DisplayInfo(string main_text, Color color, float length) {
            this.main_text = main_text;
            this.color = color;
            this.length = length;
            sprite = null;
            sub_text = null;
        }
        public DisplayInfo(string main_text, string sub_text, Color color, float length) {
            this.main_text = main_text;
            this.color = color;
            this.length = length;
            sprite = null;
            this.sub_text = sub_text;
        }
        public DisplayInfo(string main_text, Sprite sprite, Color color, float length) {
            this.main_text = main_text;
            this.color = color;
            this.length = length;
            this.sprite = sprite;
            sub_text = null;
        }
        public DisplayInfo(string main_text, string sub_text, Sprite sprite, Color color, float length) {
            this.main_text = main_text;
            this.color = color;
            this.length = length;
            this.sprite = sprite;
            this.sub_text = sub_text;
        }
        public string main_text, sub_text;
        public Color color;
        public Sprite sprite;
        public float length;
    }
}
