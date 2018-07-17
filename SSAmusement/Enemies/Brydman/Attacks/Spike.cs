using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class Spike : MonoBehaviour {

    public bool can_spike {
        get; private set;
    }

    Animator anim;
    SpriteRenderer sr;

    [SerializeField] Attack big_spike;
    [SerializeField] float distance_to_shoot, shoot_speed, retract_speed;
    float x_position;
    bool spike_out;
    
    // Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        spike_out = false;
	}

    void Update() {
        can_spike = anim.GetCurrentAnimatorStateInfo(0).IsName("SpikeReadyIdle") && !spike_out;
    }

    public void SetOnHit(Attack.OnHit on_hit) {
        big_spike.SetOnHit(on_hit);
    }

    public void Load() {
        anim.SetBool("Ready", true);
    }

    public void ShootSpike(float length) {
        StartCoroutine(ShootSpikeRoutine(length));
    }

    IEnumerator ShootSpikeRoutine(float length) {
        spike_out = true;
        anim.SetBool("Out", true);
        big_spike.gameObject.SetActive(true);
        x_position = big_spike.transform.position.x;
        big_spike.Enable();
        float distance = 0;
        while (distance_to_shoot > distance) {
            float to_move = shoot_speed * Time.deltaTime;
            if (distance_to_shoot < distance + to_move) {
                to_move = distance_to_shoot - distance;
            }
            distance += to_move;
            big_spike.transform.localPosition += Vector3.left * to_move;
            yield return null;
        }
        big_spike.Disable();

        yield return new WaitForSeconds(length);

        while (distance > 0) {
            float to_move = retract_speed * Time.deltaTime;
            if (0 > distance - to_move) {
                to_move = distance;
            }
            distance -= to_move;
            big_spike.transform.localPosition += Vector3.right * to_move;
            yield return null;
        }
        big_spike.transform.position = new Vector3(x_position, big_spike.transform.position.y, big_spike.transform.position.z);
        big_spike.gameObject.SetActive(false);
        anim.SetBool("Out", false);
        spike_out = false;
    }

    public void Unload() {
        anim.SetBool("Ready", false);
    }
}
