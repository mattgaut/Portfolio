using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RocketSpawner : MonoBehaviour {

    [SerializeField] HomingProjectile rocket;
    Transform homing_target;

    [SerializeField] Transform spawn_point;
    Attack.OnHit on_hit;

    Animator anim;
    public bool can_spawn {
        get; private set;
    }

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    public void Update() {
        can_spawn = anim.GetCurrentAnimatorStateInfo(0).IsName("RocketReadyIdle");
    }

    public void Ini(HomingProjectile rocket_prefab, Attack.OnHit on_hit, Transform target) {
        rocket = rocket_prefab;
        this.on_hit = on_hit;
        homing_target = target;
    }

    public void Ready() {
        anim.SetTrigger("Ready");
    }

    public HomingProjectile Spawn() {
        if (can_spawn) {
            anim.SetTrigger("Spawn");

            HomingProjectile new_rocket = Instantiate(rocket, spawn_point.position, transform.rotation);
            new_rocket.SetTarget(homing_target);
            new_rocket.GetComponent<Attack>().SetOnHit(on_hit);
            return new_rocket;
        }
        return null;
    }
}
