using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : Item {

    [SerializeField]
    float boostMagnitude, length, fade_length;

    public override void OnUse(Player user) {
        Rigidbody body = user.GetComponent<Rigidbody>();

        user.StartCoroutine(BoostOverTime(user));
    }

    IEnumerator BoostOverTime(Player user) {
        Rigidbody body = user.GetComponent<Rigidbody>();

        float time = length;
        user.attached_ui.item_timer.SpawnTimer(sprite, Color.green, length);
        while (time > 0) {
            time -= Time.deltaTime;

            Vector3 vec = user.transform.rotation * Vector3.forward;
            vec = vec * boostMagnitude;
            if (Time.timeScale != 0)
                body.AddForce(vec, ForceMode.Impulse);

            yield return null;
        }
        time = fade_length;
        while (time > 0) {
            time -= Time.deltaTime;

            Vector3 vec = user.transform.rotation * Vector3.forward;
            vec = vec * boostMagnitude;
            if (Time.timeScale != 0)
                body.AddForce(vec * (time/fade_length), ForceMode.Impulse);

            yield return null;
        }
    }

    public static IEnumerator BoostOverTime(Player user, float length, float fade_length, float boostMagnitude) {
        Rigidbody body = user.GetComponent<Rigidbody>();

        float time = length;
        while (time > 0) {
            time -= Time.deltaTime;

            Vector3 vec = user.transform.rotation * Vector3.forward;
            vec = vec * boostMagnitude;
            if (Time.timeScale != 0)
                body.AddForce(vec, ForceMode.Impulse);

            yield return null;
        }
        time = fade_length;
        while (time > 0) {
            time -= Time.deltaTime;

            Vector3 vec = user.transform.rotation * Vector3.forward;
            vec = vec * boostMagnitude;
            if (Time.timeScale != 0)
                body.AddForce(vec * (time / fade_length), ForceMode.Impulse);

            yield return null;
        }
    }

}
