using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveyardContainer : CardContainer {
    [SerializeField]
    float card_scale;

    protected override void UpdateView(bool adding_card = true) {

        if (cards.Count != 0) {
            cards[cards.Count - 1].SetView(2);
            LerpToPosition(cards[cards.Count - 1], transform.position + new Vector3(0,1,0));
            cards[cards.Count - 1].transform.localScale = (new Vector3(card_scale, 0, card_scale));
        }
        StartCoroutine(After());
    }
    IEnumerator After() {
        float timer = 0.25f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        foreach (Card c in cards) {
            if (c != cards[cards.Count - 1]) {
                LerpToPosition(c, transform.position);
                c.SetView(0);
            }
            c.transform.position = transform.position;
        }
    }
}
