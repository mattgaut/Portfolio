using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEditorController : MonoBehaviour {

    [SerializeField] float zoom_speed, pan_speed;
    Camera cam;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Q)) {
            cam.orthographicSize += zoom_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E)) {
            cam.orthographicSize -= zoom_speed * Time.deltaTime;
        }
        transform.position += new Vector3(0, pan_speed * Input.GetAxisRaw("Vertical")* Time.deltaTime, 0);
        transform.position += new Vector3(pan_speed * Input.GetAxisRaw("Horizontal") * Time.deltaTime, 0);
    }
}
