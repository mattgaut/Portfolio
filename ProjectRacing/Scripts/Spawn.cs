using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    [SerializeField]
    GameObject spawn;

    void Awake() {
        GameObject new_spawn = Instantiate(spawn, transform.parent);
        new_spawn.transform.position = transform.position;

        Destroy(gameObject);
    }
}
