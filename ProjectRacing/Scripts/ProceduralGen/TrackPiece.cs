using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TrackPiecePosition))]
public class TrackPiece : MonoBehaviour {

    [SerializeField]
    TrackPiecePosition _track_position;
    public TrackPiecePosition track_position {
        get { return _track_position; }
    }

    [SerializeField]
    TrackPieceObstacles _obstacles;
    public TrackPieceObstacles obstacles {
        get { return _obstacles; }
    }

    bool entered = false;

    [SerializeField]
    MeshRenderer rend;

    public void SetColor(Material m) {

        for (int i = 0; i < rend.materials.Length; i++) {
            rend.materials[i].CopyPropertiesFromMaterial(m);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player") {
            TrackManager.instance.CheckVisit(this, collider.transform.root.GetComponent<Player>());
            entered = true;
        }
    }
}
