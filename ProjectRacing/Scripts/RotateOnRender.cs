using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnRender : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void OnWillRenderObject() {
        transform.rotation = Camera.current.transform.rotation;
    }
}
