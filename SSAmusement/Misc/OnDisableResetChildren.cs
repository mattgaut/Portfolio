using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDisableResetChildren : MonoBehaviour {

    [SerializeField] List<GameObject> active;

    private void OnDisable() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(active.Contains(transform.GetChild(i).gameObject));
        }
    }
}
