using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class PowerUp : MonoBehaviour {

    Sprite _icon;
    public Sprite icon {
        get { return _icon; }
    }

    protected float timer;
    protected bool countdown;

    SpriteRenderer sr;

    Collider2D coll;

    public abstract float length {
        get;
    }

    private void Awake() {
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        _icon = sr.sprite;
    }

    protected void Update() {
        if (countdown) {
            timer -= Time.deltaTime;
            if (timer < 0) {
                BeforeDestroy();
                Destroy(gameObject);
            }
        }
    }

    protected virtual void BeforeDestroy() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            AddPowerup(collision.gameObject.GetComponentInParent<Player>());
        }
    }

    protected virtual void AddPowerup(Player p) {
        p.TrackPowerUp(this);
        coll.enabled = false;
        sr.enabled = false;
        countdown = true;

        timer = length;
    }
}
