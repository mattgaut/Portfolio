using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvokeButtonOnKeypress : MonoBehaviour {

    [SerializeField]
    Button b;
    [SerializeField]
    string key;

    void Update() {
        if (Input.GetButtonDown(key)) {
            b.onClick.Invoke();
        }
    }
}
