using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterRotate : MonoBehaviour {
	
	void LateUpdate () {
        transform.eulerAngles = Vector3.zero;
	}
}
