using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HealthPickup : MonoBehaviour {

    [SerializeField] float health_restored;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            Player p = collision.gameObject.GetComponentInParent<Player>();
            p.RestoreHealth(health_restored);
            Destroy(gameObject);
        }
    }
}
