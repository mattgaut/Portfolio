using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTrigger : MonoBehaviour {

    [SerializeField]
    Text card_name, description;
    [SerializeField]
    Image art, highlight;

    TriggeredAbility.TriggerInstance display;

    public virtual void Display(TriggeredAbility.TriggerInstance card) {
        display = card;
        card_name.text = "From: " + card.ability.source.card_name;
        description.text = card.description;
        art.sprite = card.ability.source.art;
    }

    Coroutine check_click;
    void OnMouseEnter() {
        GameManager.instance.Hover(true, display);
        check_click = StartCoroutine(CheckClick());
    }
    void OnMouseExit() {
        if (check_click != null) {
            StopCoroutine(check_click);
            check_click = null;
        }
        GameManager.instance.Hover(false, display);
    }
    IEnumerator CheckClick() {
        while (true) {
            while (!Input.GetMouseButtonDown(0)) {
                yield return null;
            }
            while (!Input.GetMouseButtonUp(0)) {
                yield return null;
            }
            ContextClickHandler();
        }
    }
    void ContextClickHandler() {
        if (GameManager.instance.targeting && GameManager.instance.targeter.controller == GameManager.instance.player1) {
            GameManager.instance.ToggleTarget(display);
        }
        GameManager.instance.Hover(false, display);
        GameManager.instance.Hover(true, display);
    }

    public void Highlight(bool highlight) {
        this.highlight.enabled = highlight;
    }
}
