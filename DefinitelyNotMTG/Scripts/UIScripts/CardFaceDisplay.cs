using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFaceDisplay : MonoBehaviour {

    [SerializeField]
    Text card_name, description, type;
    [SerializeField]
    Image art, highlight;
    public Image highlight_image {
        get { return highlight; }
    }

    [SerializeField]
    GameObject red, yellow, blue, black, green, generic;

    [SerializeField]
    GameObject mana_cost;

    public virtual void Display(Card card) {
        card_name.text = card.card_name;
        description.text = GetText(card);
        if (mana_cost != null && (card.GetType() == typeof(Spell) || card.GetType().IsSubclassOf(typeof(Spell))))
            SetManaCost((card as Spell).mana_cost);
        else
            ClearManaCost();
        type.text = CardTypeToString(card.major_card_type);

        art.sprite = card.art;

        highlight.enabled = false;
    }

    string CardTypeToString(MajorCardType type) {
        if (type == MajorCardType.instant) {
            return "Instant";
        }
        if (type == MajorCardType.creature) {
            return "Creature";
        }
        if (type == MajorCardType.sorcery) {
            return "Sorcery";
        }
        if (type == MajorCardType.mana) {
            return "Mana";
        }
        if (type == MajorCardType.structure) {
            return "Structure";
        }
        return "";
    }

    void ClearManaCost() {
        for (int i = 0; i < mana_cost.transform.childCount; i++) {
            Destroy(mana_cost.transform.GetChild(i).gameObject);
        }
    }
    void SetManaCost(ManaCost cost) {
        ClearManaCost();
        if (cost.generic > 0) {
            GameObject g = Instantiate(generic, mana_cost.transform, false);
            g.GetComponentInChildren<Text>().text = "" + cost.generic;
        }
        for (int i = 0; i < cost.green; i++) {
            Instantiate(green, mana_cost.transform, false);
        }
        for (int i = 0; i < cost.red; i++) {
            Instantiate(red, mana_cost.transform, false);
        }
        for (int i = 0; i < cost.black; i++) {
            Instantiate(black, mana_cost.transform, false);
        }
        for (int i = 0; i < cost.blue; i++) {
            Instantiate(blue, mana_cost.transform, false);
        }
        for (int i = 0; i < cost.yellow; i++) {
            Instantiate(yellow, mana_cost.transform, false);
        }
    }

    public void Highlight(bool highlight) {
        this.highlight.enabled = enabled;
    }

    string GetText(Card c) {
        string s = "";
        if (c.triggers != null) {
            if (c.triggers.keywords.flying) {
                s += "Flying";
            }
            if (c.triggers.keywords.first_strike) {
                if (s != "") s += ", ";
                s += "First Strike";
            }
            if (c.triggers.keywords.lifelink) {
                if (s != "") s += ", ";
                s += "Lifelink";
            }
            if (c.triggers.keywords.haste) {
                if (s != "") s += ", ";
                s += "Haste";
            }
            if (c.triggers.keywords.unblockable) {
                if (s != "") s += ", ";
                s += "Unblockable";
            }
            if (c.triggers.keywords.defender) {
                if (s != "") s += ", ";
                s += "Defender";
            }
            if (c.triggers.keywords.deathtouch) {
                if (s != "") s += ", ";
                s += "Deathtouch";
            }
            if (s != "") {
                s += "\n";
            }
        }
        return s + c.card_description;
    }
}
