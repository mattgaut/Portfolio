using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithAspectRatio : MonoBehaviour {
    [SerializeField]
    Camera to_scale;
    [SerializeField]
    float base_aspect;

	// Use this for initialization
	void Start () {
        //float horizontal = to_scale.orthographicSize * base_aspect;
        //to_scale.orthographicSize = horizontal / to_scale.aspect;

        //Debug.Log(to_scale.aspect);
        to_scale.aspect = base_aspect;
	}
}
