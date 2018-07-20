using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : Enemy {

    [SerializeField]
    LayerMask bounces_off;
    [SerializeField]
    SplitterHolder holder;

    [SerializeField]
    GameObject spawn;

    [SerializeField]
    int _level;
    public int level {
        get { return _level; } set { _level = value; }
    }

    public Vector3 movement {
        get; protected set;
    }

    public void SetSpawn(GameObject spawn) {
        this.spawn = spawn;
    }

    protected override void Start() {
        base.Start();
        if (movement == null || movement == Vector3.zero)
            movement = Quaternion.Euler(0, 0, 45 + (90 * Random.Range(0, 4))) * Vector3.up;

        transform.localScale = Vector3.one * (level);
        if (holder == null) {
            holder = transform.parent.GetComponent<SplitterHolder>();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D coll) {
        base.OnCollisionEnter2D(coll);
        if ((1 << coll.gameObject.layer & bounces_off) != 0) {
            Vector2 vec = coll.contacts[0].point - (Vector2)transform.position;
            Vector3 euelers = Quaternion.LookRotation(Vector3.forward, (Vector3)vec).eulerAngles;
            if ((euelers.z < 45 || euelers.z > 315)) {
                if (movement.y > 0) {
                    movement = new Vector3(movement.x, -movement.y, 0);
                }
            } else if (euelers.z > 135 && euelers.z < 225) {
                if (movement.y < 0) {
                    movement = new Vector3(movement.x, -movement.y, 0);
                }
            } else if (euelers.z > 225 && euelers.z < 315) {
                if (movement.x > 0) {
                    movement = new Vector3(-movement.x, movement.y, 0);
                }
            } else if (euelers.z > 45 && euelers.z < 135) {
                if (movement.x < 0) {
                    movement = new Vector3(-movement.x, movement.y, 0);
                }
            }
        }
    }
    protected void OnTriggerEnter2D(Collider2D coll) {
        if ((1 << coll.gameObject.layer & 1 << LayerMask.NameToLayer("Player")) != 0) {
            coll.GetComponent<PlayerCharacter>().TakeDamage(1);
        }
    }

    public override float TakeDamage(float dmg) {
        if (!dead && health > 0) {

            if (health < dmg) {
                holder.TakeDamage(health);
            } else {
                holder.TakeDamage(dmg);
            }
        }
        return base.TakeDamage(dmg);
    }

    protected override void Die() {
        //if (!dead) {
        //    holder.TakeDamage(max_health.value);
        //}
        if (level > 1 && !dead) {
            dead = true;
            Split();
        } else {
            if (gameObject != null && !dead) {
                dead = true;
                if (room && room.enemies_in_room.Contains(this))
                    room.RemoveEnemy(this);
                if (holder.CheckSpawnObjects()) {
                    foreach (GameObject spawn in spawn_on_death) {
                        Instantiate(spawn, transform.position, Quaternion.identity);
                    }
                }
                gameObject.SetActive(false);
            }
        }
    }

    protected void Split() {
        if (room && room.enemies_in_room.Contains(this))
            room.RemoveEnemy(this);

        setspeed = 1.5f * setspeed;
        transform.localScale /= 2;
        Splitter split_1 = Instantiate(spawn, transform.position, Quaternion.identity).GetComponent<Splitter>();
        Splitter split_2 = Instantiate(spawn, transform.position, Quaternion.identity).GetComponent<Splitter>();

        split_1.setspeed = setspeed;
        //split_1.transform.localScale = transform.localScale;
        split_1.movement = Quaternion.Euler(0, 0, 90) * movement;
        split_1.spawn = spawn;
        split_1.level = level - 1;
        split_1.holder = holder;
        split_1.sethealth = sethealth / 2;

        split_2.setspeed = setspeed;
        //split_2.transform.localScale = transform.localScale;
        split_2.movement = Quaternion.Euler(0, 0, 270) * movement;
        split_2.spawn = spawn;
        split_2.level = level - 1;
        split_2.holder = holder;
        split_2.sethealth = sethealth / 2;

        split_1.transform.SetParent(transform.parent, true);
        split_2.transform.SetParent(transform.parent, true);
        Destroy(gameObject);
    }

    public float GetStartingHealth() {
        return sethealth;
    }
}
