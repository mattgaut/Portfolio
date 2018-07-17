using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoHideMouse : MonoBehaviour {

    [SerializeField] float time_until_hide;

    Vector3 last_position;
    float timer;

    private void Start() {
        timer = 0;
    }

    void Update () {
		if (Input.mousePosition == last_position) {
            timer += Time.deltaTime;
            if (timer >= time_until_hide) {
                Cursor.visible = false;
            }
        } else {
            last_position = Input.mousePosition;
            timer = 0;
            Cursor.visible = true;
        }
	}
}
