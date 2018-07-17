using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    TrackManager manager;
    [SerializeField] Player player;
    [SerializeField] GameObject pointer;

    float timer = 0f;

    void Awake() {
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player" + (player.player_number + 1) + "Only"));
    }

    void Start() {
        manager = TrackManager.instance;
    }

    void Update() {
        if (manager != null) {
            TrackPiece piece = manager.LastPiece(player);

            if (piece != null) {
                Vector2 end_position = piece.track_position.end_position.position * piece.transform.localScale.x;

                Vector3 point_at = new Vector3(end_position.x, 0, end_position.y);

                Quaternion rotation = Quaternion.LookRotation(point_at - transform.position);

                rotation = Quaternion.Euler(new Vector3(transform.rotation.x, rotation.eulerAngles.y, transform.rotation.z));

                transform.rotation = rotation;

                if (Mathf.DeltaAngle(rotation.eulerAngles.y, transform.root.rotation.eulerAngles.y) > 90f) {
                    timer += Time.deltaTime;
                    if (timer > 2f) {
                        pointer.SetActive(true);
                    }
                } else {
                    timer = 0;
                    pointer.SetActive(false);
                }
            }
        }
    }

    void SetLayerRecursively(GameObject obj, int new_layer) {
        if (null == obj) {
            return;
        }

        obj.layer = new_layer;

        foreach (Transform child in obj.transform) {
            if (null == child) {
                continue;
            }
            SetLayerRecursively(child.gameObject, new_layer);
        }
    }
}
