using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour {

    [SerializeField]
    Text health_text;
    [SerializeField]
    Image highlight;

    public void Awake() {
        highlight.enabled = false;
    }

    public void Display(Player p) {
        health_text.text = p.life + "";
    }

    public void Highlight(bool highlight) {
        this.highlight.enabled = highlight;
    }
}
