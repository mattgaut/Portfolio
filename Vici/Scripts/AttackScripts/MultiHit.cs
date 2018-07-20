using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiHit : Attack {

    [SerializeField]
    float tick_rate;

    protected List<IDamageable> hit_objects;

    float counter;
    Coroutine ticker;

    protected override void Awake() {
        base.Awake();
        hit_objects = new List<IDamageable>();
        counter = tick_rate;
        ticker = StartCoroutine(TickCounter());
    }

    protected override void TriggerEnter(IDamageable hit_obj) {
        if (!hit_objects.Contains(hit_obj)) {
            hit_objects.Add(hit_obj);
        }
    }

    protected override void TriggerExit(IDamageable hit_obj) {
        if (hit_objects.Contains(hit_obj)) {
            hit_objects.Remove(hit_obj);
        }
    }

    public void SetTickRate(float tick_rate) {
        this.tick_rate = tick_rate;
    }

    protected virtual IEnumerator TickCounter() {
        while (true) {
            counter += Time.deltaTime;
            if (counter >= tick_rate) {
                counter -= tick_rate;
                for(int i = 0; i < hit_objects.Count; i++) {
                    if (hit_objects[i] == null || hit_objects[i].immune) {
                        hit_objects.RemoveAt(i);
                        i--;
                    } else {
                        onHit(hit_objects[i]);
                    }
                }
            }
            yield return null;
        }
    }

    float time_disabled = -1;
    protected override void OnEnable() {
        base.OnEnable();
        if (ticker != null) {
            StopCoroutine(ticker);
        }
        if (Time.time - time_disabled >= tick_rate || time_disabled == -1) {
            counter = tick_rate;
        } else {
            counter = Time.time - time_disabled;
        }
        ticker = StartCoroutine(TickCounter());
    }

    protected virtual void OnDisable() {
        hit_objects.Clear();
        if (ticker != null) {
            StopCoroutine(ticker);
        }
        time_disabled = Time.time;
    }
}
