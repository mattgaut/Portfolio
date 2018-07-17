using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class speedPadScript : MonoBehaviour {

    public float boostMagnitude = 2500.0f;
    public Vector3 boostForce;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            Rigidbody body = other.transform.root.gameObject.GetComponent<Rigidbody>();

            Vector3 vec = body.transform.rotation * Vector3.forward;
            vec = vec * boostMagnitude;
            body.AddForce(vec, ForceMode.Impulse);
        }
    }
}
