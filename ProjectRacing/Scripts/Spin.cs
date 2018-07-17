using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

    [SerializeField]
    float spin_x, spin_y, spin_z;
    Vector3 spin;

    void Awake() {
        spin = new Vector3(spin_x, spin_y, spin_z);
    }

	void Update () {
        transform.rotation *= Quaternion.Euler(spin * Time.deltaTime);
	}
}
