using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour {

    public GameObject car;
    private CarController carScript;
    private WheelCollider[] wheels;

    [Range(0.0f,180.0f)]
    public float zoomScale;

    public float maxViewAngle;

    private Vector3 offset;
    private Camera camComponent;

    public Vector3 cameraOrientation;

    void Start ()
    {
        camComponent = GetComponent<Camera>();
        camComponent.fieldOfView = zoomScale;
        offset = this.transform.position - car.transform.position;
        cameraOrientation = this.transform.eulerAngles;

        carScript = car.GetComponent<CarController>();
        //wheels = carScript.WheelColliders;
        Assert.IsNotNull(wheels, "Wheels in CameraScript are null");
	}

    //goal: is to clamp the camera's rotation about the y axis.
    void LateUpdate ()
    {
        transform.position = car.transform.position + offset;
        this.transform.rotation.SetLookRotation(this.transform.position - car.transform.position);
    }
}
