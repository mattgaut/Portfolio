using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    [SerializeField]
    protected float sethealth, setspeed;
    [SerializeField]
    bool damages_on_contact;

    protected List<GameObject> spawn_on_death;

    public RoomLayout room {
        get; private set;
    }
    protected override void Awake() {
        base.Awake();

        spawn_on_death = new List<GameObject>();
    }

    protected override void Start() {
        base.Start();
        max_health = new BaseStat(sethealth);
        health = sethealth;
        speed = new BaseStat(setspeed);
    }

    public override float TakeDamage(float dmg) {
        if (dead) return 0;
        float ret = base.TakeDamage(dmg);
        if (dmg > 0) {
            if (animator && !dead)
                animator.SetTrigger("DamageTrigger");
        }
        return ret;
    }

    protected virtual void OnCollisionEnter2D(Collision2D coll) {
        if (!stunned) CheckHit(coll.gameObject);
    }
    protected virtual void OnCollisionStay2D(Collision2D coll) {
        if (!stunned) CheckHit(coll.gameObject);
    }

    protected virtual void CheckHit(GameObject g) {
        if (damages_on_contact && (1 << LayerMask.NameToLayer("Player") & 1 << g.layer) != 0) {
            g.GetComponent<PlayerCharacter>().TakeDamage(1);
        }
    }

    protected override void Die() {
        if (gameObject != null) {
            dead = true;
            if (room && room.enemies_in_room.Contains(this))
                room.RemoveEnemy(this);
            foreach (GameObject spawn in spawn_on_death) {
                room.DropInRoom(spawn, transform.position);
            }
            gameObject.SetActive(false);
        }
    }
    public void SetRoom(RoomLayout r) {
        room = r;
    }

    public void AddPickup(GameObject go) {
        spawn_on_death.Add(go);
    }
}
