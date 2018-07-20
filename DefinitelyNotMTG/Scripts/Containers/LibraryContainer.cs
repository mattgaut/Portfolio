using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryContainer : CardContainer {

    [SerializeField]
    Text library_count;

    protected override void UpdateView(bool adding_card = true) {
        foreach (Card c in cards) {
            c.transform.position = transform.position + Vector3.up * 5;
        }
        library_count.text = "" + cards.Count;
    }
}
