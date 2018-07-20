using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAwakeDisplay : MonoBehaviour {

    [SerializeField]
    CreatureFaceDisplay display;
    [SerializeField]
    Card c;

    void Start() {
        display.Display(c);
    }
}
