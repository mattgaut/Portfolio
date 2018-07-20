using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandContainer : CardContainer {

    [SerializeField]
    float card_scale, horizontal_offset;

    [SerializeField]
    float max_width;

    protected override void UpdateView(bool adding_card = true) {
        if ((1.75f + horizontal_offset) * (cards.Count - 1) * card_scale < max_width) {
            for (int i = 0; i < cards.Count; i++) {
                LerpToPosition(cards[i], transform.position + new Vector3((1.75f + horizontal_offset) * (i) * card_scale, 0, 0));
                cards[i].transform.localScale = new Vector3(card_scale, 1, card_scale);
            }
        } else {
            for (int i = 0; i < cards.Count; i++) {
                LerpToPosition(cards[i], transform.position + new Vector3(max_width * ((float)(i)/(cards.Count - 1)), .1f * (i), 0));
                cards[i].transform.localScale = new Vector3(card_scale, 1, card_scale);
            }
        }

    }
}
