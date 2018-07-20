using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    [SerializeField]
    GameObject followobj;
    [SerializeField]
    float offset;
    [SerializeField]
    bool use_mouse_position;
    [SerializeField][Range(0, 0.5f)]
    float max_left_offset, max_right_offset, max_top_offset, max_bottom_offset;



    // Use this for initialization
    void Start() {
        if (followobj == null) {
            followobj = GameObject.FindWithTag("Player");
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if (followobj == null) {
            followobj = GameObject.FindWithTag("Player");
        }

        if (followobj != null) {

            float x = transform.position.x, y = transform.position.y;
            if (transform.position.x - followobj.transform.position.x > offset) {
                x = followobj.transform.position.x + offset;
            }
            if (transform.position.x - followobj.transform.position.x < -offset) {
                x = followobj.transform.position.x - offset;
            }
            if (transform.position.y - followobj.transform.position.y > offset) {
                y = followobj.transform.position.y + offset;
            }
            if (transform.position.y - followobj.transform.position.y < -offset) {
                y = followobj.transform.position.y - offset;
            }

            transform.position = new Vector3(x, y, transform.position.z);

            if (use_mouse_position && Input.GetButton("Jump")) {
                Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float move_y_percent = ((Input.mousePosition.y - ((float)Camera.main.pixelHeight / 2)) / Camera.main.pixelHeight);
                float move_x_percent = ((Input.mousePosition.x - ((float)Camera.main.pixelWidth / 2)) / Camera.main.pixelWidth);

                Vector3 full = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0)) - Camera.main.ScreenToWorldPoint(new Vector3(0, 0));

                if (move_y_percent > max_top_offset) {
                    move_y_percent = max_top_offset;
                }
                if (move_y_percent < -max_bottom_offset) {
                    move_y_percent = -max_bottom_offset;
                }
                if (move_x_percent > max_right_offset) {
                    move_x_percent = max_right_offset;
                }
                if (move_x_percent < -max_left_offset) {
                    move_x_percent = -max_left_offset;
                }

                transform.position += new Vector3(full.x * move_x_percent, full.y * move_y_percent);
            }
        }
    }
}
