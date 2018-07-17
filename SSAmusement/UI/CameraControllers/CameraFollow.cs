using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] Transform follow;
    [SerializeField] Room boundary_room;

    [SerializeField] bool grab_player;

    Vector3 new_position;
    bool lerping;

    private void Start() {
        lerping = false;
        if (grab_player) {
            follow = FindObjectOfType<Player>().transform;
        }
    }

    private void LateUpdate() {
        if (!lerping) {
            new_position = follow.transform.position + 10 * Vector3.back;
            if (boundary_room != null) {
                new_position = boundary_room.ClampToCameraBounds(new_position);
            } else if (RoomManager.instance != null && RoomManager.instance.active != null) {
                new_position = RoomManager.instance.active.ClampToCameraBounds(new_position);
            }
        }
        transform.position = new_position;
    }

    public void LerpToTransform(Transform t, float time) {
        lerping = true;
        StartCoroutine(Lerp(t, time));
    }

    IEnumerator Lerp(Transform t, float time) {
        float timer = 0;
        Vector3 original_position = transform.position;
        new_position = original_position;
        while (timer < time) {
            timer += Time.deltaTime;
            new_position = original_position + ((Clamped(t.position) - original_position) * (timer / time));
            new_position.z = -10;
            yield return null;
        }
        new_position = Clamped(t.position) + (10 * Vector3.back);
        lerping = false;
    }

    Vector3 Clamped(Vector3 position) {
        if (boundary_room != null) {
            return boundary_room.ClampToCameraBounds(position);
        } else if (RoomManager.instance != null && RoomManager.instance.active != null) {
            return RoomManager.instance.active.ClampToCameraBounds(position);
        } else {
            return position;
        }
    }
}
