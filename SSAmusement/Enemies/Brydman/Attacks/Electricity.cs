using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Attack), typeof(SpriteRenderer))]
public class Electricity : MonoBehaviour {

    Animator anim;
    Attack attack;
    SpriteRenderer sr;
    int sorting_order;
	
    public bool can_begin_zap {
        get; private set;
    }
    // Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        attack = GetComponent<Attack>();
        sr = GetComponent<SpriteRenderer>();
        sorting_order = sr.sortingOrder;
    }

    private void Update() {
        can_begin_zap = anim.GetCurrentAnimatorStateInfo(0).IsName("ElectricityRaisedIdle");
    }

    public void Raise() {
        anim.SetBool("Raise",true);
    }

    public void Ready() {
        sr.sortingOrder = sr.sortingOrder + 1;
        anim.SetTrigger("Ready");
    }

    public void Zap(float length) {
        StartCoroutine(ZapRoutine(length));
    }

    IEnumerator ZapRoutine(float length) {
        anim.SetBool("Zap", true);
        attack.Enable();
        yield return new WaitForSeconds(length);
        attack.Disable();
        sr.sortingOrder = sorting_order;
        anim.SetBool("Zap", false);
    }

    public void Lower() {
        anim.SetBool("Raise", false);
    }

    public void SetOnHit(Attack.OnHit on_hit) {
        attack.SetOnHit(on_hit);
    }
}
