using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldCardDisplay : MonoBehaviour {
    [SerializeField]
    Image highlight, fade, summoning_sick, art;
    public Image highlight_image {
        get { return highlight; }
    }

    void Awake() {
        highlight.enabled = false;
    }

    public virtual void Display(Card c) {
        SpellPermanent display = c as SpellPermanent;
        if (display == null) return;

        art.sprite = c.art;

        fade.enabled = display.exhausted;
        summoning_sick.enabled = display.summoning_sick;
    }

    public void Highlight(bool highlight) {
        this.highlight.enabled = highlight;
    }
}
