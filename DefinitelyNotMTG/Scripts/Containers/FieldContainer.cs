using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldContainer : CardContainer {
    [SerializeField]
    float card_scale, offset;

    protected override void UpdateView(bool adding_card = true) {
        for (int i = 0; i < cards.Count; i++) {
            if (adding_card && i == cards.Count - 1)
                SetPosition(cards[i], transform.position + new Vector3((1.25f + offset) * i, 0, 0));
            else
                LerpToPosition(cards[i], transform.position + new Vector3((1.25f + offset) * i, 0, 0));
            cards[i].transform.localScale = new Vector3(card_scale, 1, card_scale);
        }
    }

    public bool HasCreatures() {
        foreach (Creature c in cards.OfType<Creature>()) {
            return true;
        }
        return false;
    }
}
