using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsDropDownHandler : MonoBehaviour {

    Dropdown dropdown;

    private void Awake() {
        dropdown = GetComponent<Dropdown>();
        if (!FPSCounter.is_on) dropdown.value = 0;
        else dropdown.value = (int)FPSCounter.position + 1;
    }

    public void Dropdown(Dropdown d) {
        if (d.value == 0) FPSCounter.SetOn(false);
        else {
            FPSCounter.SetPosition((FPSCounter.Position)(d.value - 1));
            FPSCounter.SetOn(true);
        }

    }

    private void OnDisable() {
        if (transform.childCount ==4) {
            Destroy(transform.GetChild(3).gameObject);
        }
    }
}
