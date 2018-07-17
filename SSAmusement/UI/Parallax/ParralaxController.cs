using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParralaxController : MonoBehaviour {

    Vector3 last_position;
    List<Parralax> to_control;

    private void Awake() {
        to_control = new List<Parralax>(GetComponentsInChildren<Parralax>());
        last_position = transform.position;
        foreach (Parralax p in to_control) {
            p.transform.SetParent(null);
        }
    }

    private void Update() {
        if (last_position != transform.position) {
            foreach (Parralax p in to_control) {
                p.UpdateOrigin(transform.position);
            }
        }
    }
}
