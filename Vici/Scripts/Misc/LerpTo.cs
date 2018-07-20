using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTo : MonoBehaviour {

    public float lerp_time_left {
        get; private set;
    }
    public float total_lerp_time {
        get; private set;
    }
    Coroutine current_lerp;

    public void LerpToPosition(Vector3 position, float time) {
        lerp_time_left = time;
        total_lerp_time = time;
        if (current_lerp != null) {
            StopCoroutine(current_lerp);
        }
        current_lerp = StartCoroutine(Lerp(position));
    }

    public IEnumerator LerpToPositionRoutine(Vector3 position, float time) {
        lerp_time_left = time;
        total_lerp_time = time;
        if (current_lerp != null) {
            StopCoroutine(current_lerp);
        }
        current_lerp = StartCoroutine(Lerp(position));
        yield return current_lerp;
    }

    IEnumerator Lerp(Vector3 position) {
        Vector3 start_position = transform.position;
        while (lerp_time_left > 0) {
            lerp_time_left -= Time.deltaTime;
            transform.position = Vector3.Lerp(start_position, position, (1f - lerp_time_left/total_lerp_time));

            yield return null;
        }
        lerp_time_left = 0;
    }
}
