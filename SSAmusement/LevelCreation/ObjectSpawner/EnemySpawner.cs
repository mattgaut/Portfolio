using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] Enemy to_spawn;
    public Enemy Spawn() {
        Enemy enemey = Instantiate(to_spawn);
        enemey.transform.SetParent(transform.parent);
        enemey.transform.position = transform.position;
        Destroy(gameObject);
        return enemey;
    }
}
