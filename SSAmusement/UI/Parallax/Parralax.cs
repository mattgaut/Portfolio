using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parralax : MonoBehaviour {

    float lambda = .05f;
    [SerializeField][Range(0, 10)] float distance;
    float percentage;

    [SerializeField] Transform reference;

    Vector2 origin, offset;
    Vector3 parent_origin;


    private void Start() {
        Set();
    }

    private void Set() {
        if (reference == null) reference = Camera.main.transform;
        origin = transform.position;

        percentage = 0;
        if (distance >= 0) percentage = 1 - Mathf.Exp(-lambda * distance);
    }

    void LateUpdate () {
        Vector2 reference_translation = (Vector2)reference.position - origin;
       
        reference_translation *= percentage;

        transform.localPosition = reference_translation + origin;
    }

    public void UpdateOrigin(Vector2 origin) {
        this.origin = origin;
    }
}
