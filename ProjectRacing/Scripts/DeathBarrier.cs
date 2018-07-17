using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrier : MonoBehaviour {


    void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.CompareTag("Player")) {
            GameObject car = coll.transform.root.gameObject;
            car.GetComponent<Player>().LoseLife();
        }
    }
}
