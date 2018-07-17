using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRight : MonoBehaviour {

    [SerializeField] float speed, reset_after;
    float timer;
    Vector3 origin;

    private void Start() {
        timer = reset_after;
        origin = transform.position;
    }

    // Update is called once per frame
    void Update () {
        transform.position += speed * Time.deltaTime * Vector3.right;
        timer -= Time.deltaTime;
        if (timer <= 0) {
            transform.position = origin;
            timer = reset_after;
        }
    }
}
