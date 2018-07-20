using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionIgnoreRotation : MonoBehaviour {

    [SerializeField]
    Vector3 offset;

	void LateUpdate () {
        transform.position = transform.parent.position + offset;
	}
}
