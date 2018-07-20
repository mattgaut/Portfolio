using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    [SerializeField]
    GameObject to_spawn;

    [SerializeField]
    GameObject parent;

    [SerializeField]
    bool spawn_on_awake;


	// Use this for initialization
	void Awake () {
        if (spawn_on_awake) {
            SpawnObject();
        }
	}

    public GameObject SpawnObject() {
        GameObject new_object = Instantiate(to_spawn);
        new_object.transform.position = transform.position;
        if (parent != null) {
            new_object.transform.SetParent(parent.transform, true);
        }
        Destroy(gameObject);
        return new_object;
    }
}
