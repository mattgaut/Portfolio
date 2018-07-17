using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour {


    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Player")) {
            c.transform.root.GetComponent<Player>().CrossFinish();
        }
    }
}
