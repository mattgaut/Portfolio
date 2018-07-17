using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Attack), typeof(Animator))]
public class Laser : MonoBehaviour {

    Attack attack;
    Animator anim;

    public bool can_fire {
        get; private set;
    }
    bool firing;

	// Use this for initialization
	void Awake () {
        attack = GetComponent<Attack>();
        anim = GetComponent<Animator>();
	}

    private void Update() {
        can_fire = anim.GetCurrentAnimatorStateInfo(0).IsName("LaserReadyIdle");
    }

    public void SetOnHit(Attack.OnHit on_hit) {
        attack.SetOnHit(on_hit);
    }
	
    public void Open() {
        anim.SetBool("Open", true);
    }
    public void Close() {
        anim.SetBool("Open", false);
    }
    public void Ready() {
        anim.SetTrigger("Ready");
    }
    public void Fire(float length) {
        StartCoroutine(FireRoutine(length));
    }

    IEnumerator FireRoutine(float length) {
        anim.SetBool("Fire", true);
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("LaserIdle")) {
            yield return null;
        }
        attack.Enable();

        yield return new WaitForSeconds(length);

        attack.Disable();
        anim.SetBool("Fire", false);
    }
}
