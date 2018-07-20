using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable {

    [SerializeField]
    protected Slider health_slider;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected Collider2D hit_box;

    public Stat max_health {
        get; protected set;
    }

    // ControlEffects
    public bool stunned {
        get; private set;
    }
    protected virtual void Awake() { }
    protected virtual void Start() {
        dead = false;
    }

    public bool dead {
        get; protected set;
    }
    float _health;
    public virtual float health {
        get { return _health; }
        protected set {
            _health = value;
            if (_health > max_health.value) {
                _health = max_health.value;
            }
            if (health_slider)
                health_slider.SetFill(_health, max_health.value);
            if (_health <= 0) {
                Die();
            }

        }
    }
    public Stat speed {
        get; protected set;
    }

    public virtual bool immune {
        get {
            return dead ? true : !hit_box.enabled;
        }
    }

    public virtual float TakeDamage(float dmg) {
        if (health > 0) {
            health -= dmg;
            return dmg;
        }
        return 0;
    }
    public virtual int Heal(int heal) {
        health += heal;
        return heal;
    }

    protected abstract void Die();

    public void SetStunned(bool stun) {
        stunned = stun;
    }

    public void SetSlider(Slider s) {
        health_slider = s;
    }
}
