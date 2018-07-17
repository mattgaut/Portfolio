using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField] GameObject to_spawn;
    public GameObject Spawn() {
        GameObject new_enemy = Instantiate(to_spawn);
        new_enemy.transform.SetParent(transform.parent);
        new_enemy.transform.position = transform.position;
        new_enemy.transform.rotation = transform.rotation;
        SpriteRenderer sr = new_enemy.GetComponent<SpriteRenderer>();
        if (sr) {
            sr.flipX = GetComponent<SpriteRenderer>().flipX;
        }
        Destroy(gameObject);
        return new_enemy;
    }
}
