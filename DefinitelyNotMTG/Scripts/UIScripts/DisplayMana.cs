using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMana : MonoBehaviour {

    [SerializeField]
    Text current, total;

    public void Display(int current, int total) {
        this.current.text = "" + current;
        this.total.text = "" + total;
    }
}
