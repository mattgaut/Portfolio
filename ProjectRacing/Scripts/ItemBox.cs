using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour {

    void OnTriggerEnter(Collider c) {
        if (c.gameObject.CompareTag("Player")) {
            Player player = c.gameObject.transform.root.gameObject.GetComponent<Player>();
            if (player.is_active) {
                bool took_item = player.AquireItem(GameManager.instance.GetRandomItem(player));

                if (took_item) gameObject.SetActive(false);
            }
        }
    }

}
