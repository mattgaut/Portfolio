using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomPanel : MonoBehaviour {

    [SerializeField]
    GameObject card_view;

    CreatureFaceDisplay display;
    [SerializeField]
    Text text;

    Card passive;

    string passive_text;

    bool passive_visible;

    public void Awake() {
        GameObject zoom_object = card_view;
        zoom_object = Instantiate(card_view);
        zoom_object.SetActive(true);
        zoom_object.GetComponent<Collider>().enabled = false;
        zoom_object.transform.SetParent(transform, true);
        zoom_object.transform.position = transform.position;
        zoom_object.transform.localScale *= 1.5f;
        display = zoom_object.GetComponent<CreatureFaceDisplay>();
        zoom_object.SetActive(false);
    }

    public void DisplayActive(Card card, string active_text = "Focused Card:") {
        display.gameObject.SetActive(true);
        display.Display(card);
        text.text = active_text;
        text.enabled = true;
    }
    public void DisplayPassive(Card card, string passive_text = "Targeting For:") {
        this.passive_text = passive_text;
        passive = card;
        display.gameObject.SetActive(true);
        display.Display(card);
        text.text = passive_text;
        text.enabled = true;
    }
    public void ClearPassive() {
        display.gameObject.SetActive(false);
        passive = null;
        text.enabled = false;
    }
    public void ClearActive() {
        if (passive != null) {
            display.Display(passive);
        } else {
            display.gameObject.SetActive(false);
            text.enabled = false;
        }
    }
}
