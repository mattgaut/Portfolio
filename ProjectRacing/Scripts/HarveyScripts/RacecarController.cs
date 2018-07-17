using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//@Author Harvey

//issue: turning the car and accelerating feels unnatural.

//prototype class ~ subject to change
[RequireComponent (typeof (Rigidbody))]
public class RacecarController : MonoBehaviour
{
    //Preference
    //I like exposing all my variables as public so I can see the values change in Unity's inspector.

    //used to round off speed when it gets extremely close to maxSpeed or 0.
    public float tolerance = 0.01f;
    public float maxSpeed;
    public float maxSteeringAngle;

    public int accelerationScale = 1;
    public int decelerationScale = 1;
    public int reverseScale = 1;
    public int brakingScale = 1;
    public int steeringScale = 1;

    [SerializeField]
    private float speed;
    [SerializeField]
    private Vector3 directionVector;//= Vector3.forward;
    [SerializeField]
    private Vector3 deltaVelocity;
    [SerializeField]
    private Vector3 currentVelocity;

    [SerializeField]
    private Quaternion rotation;
    [SerializeField]
    private float degrees;
    [SerializeField]
    private ControllerInputs inputs;

    private Rigidbody rb;

	void Start ()
    {
        //when script attached to cube.
        rb = GetComponent<Rigidbody>();
        //when script attached to pivot point ~ so that turning happens in front of the car.
       // rb = transform.GetChild(0).GetComponent<Rigidbody>();
        degrees = Quaternion.Angle(rb.rotation, Quaternion.AngleAxis(0.0f, Vector3.up));
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            inputs.isAccelerating = true;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (speed < 0.0f)
                inputs.isBraking = true;
            else
            {
                inputs.isBraking = false;
                inputs.isReversing = true;
            }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            inputs.isSteeringLeft = true;
        }
 
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            inputs.isSteeringRight = true;
        }

        //-------------------------------------------------------------------------

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            inputs.isAccelerating = false;
        }

        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            inputs.isBraking = false;
            inputs.isReversing = false;
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            inputs.isSteeringLeft = false;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            inputs.isSteeringRight = false;
        }
    }

    void FixedUpdate()
    {
        if (inputs.isBraking)
        {
            inputs.isAccelerating = false;
            speed = Mathf.Lerp(speed, 0, Time.fixedDeltaTime * brakingScale);
            if (speed > -tolerance)
                speed = 0.0f;
        }

        if(inputs.isReversing)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.fixedDeltaTime * reverseScale);
            if (speed >= maxSpeed - tolerance)
                speed = maxSpeed;
        }

        if (inputs.isAccelerating)
        {
            speed = Mathf.Lerp(speed, -maxSpeed, Time.fixedDeltaTime * accelerationScale);
            if (speed <= -maxSpeed + tolerance)
                speed = -maxSpeed;
        }

        if (!inputs.isBraking && !inputs.isAccelerating && !inputs.isReversing)//let go of the gas pedal... slowly decelerate.
        {
            speed = Mathf.Lerp(speed, 0, Time.fixedDeltaTime * decelerationScale);
            if (speed > -tolerance)
                speed = 0.0f;
        }

        if(inputs.isSteeringLeft)
        {
            Quaternion steerQuat = Quaternion.Lerp(rb.rotation, Quaternion.AngleAxis(-maxSteeringAngle, Vector3.up), Time.fixedDeltaTime * steeringScale);
            degrees = Quaternion.Angle(steerQuat, Quaternion.AngleAxis(0.0f, Vector3.up));
            rb.MoveRotation(steerQuat);
            directionVector.z = Mathf.Cos(-degrees * Mathf.Deg2Rad);
            directionVector.x = Mathf.Sin(-degrees * Mathf.Deg2Rad);
            rotation = rb.rotation;
        }
        else if(inputs.isSteeringRight)
        {
            Quaternion steerQuat = Quaternion.Lerp(rb.rotation, Quaternion.AngleAxis(maxSteeringAngle, Vector3.up), Time.fixedDeltaTime * steeringScale);
            degrees = Quaternion.Angle(steerQuat, Quaternion.AngleAxis(0.0f, Vector3.up));
            rb.MoveRotation(steerQuat);
            directionVector.z = Mathf.Cos(degrees * Mathf.Deg2Rad);
            directionVector.x = Mathf.Sin(degrees * Mathf.Deg2Rad);
            rotation = rb.rotation;
        }

        deltaVelocity = speed * directionVector * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + deltaVelocity);
    }

    [System.Serializable]
    public struct ControllerInputs
    {
        public bool isAccelerating;
        public bool isBraking;
        public bool isReversing;
        public bool isSteeringLeft;
        public bool isSteeringRight;
    }

}
