using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used to visualize blocks in space that a track piece occupies
public class GenerateCubes : MonoBehaviour {
    [SerializeField]
    TrackPiecePosition tp;

	// Use this for initialization
	void Start () {

        foreach (TrackPosition pos in tp.TilesOccupied()) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(transform);
            int height = 0;
            if (pos.GetHeight() == TrackPosition.Height.over) {
                height = 1;
            }
            if (pos.GetHeight() == TrackPosition.Height.under) {
                height = -1;
            }
            cube.transform.position = new Vector3(pos.position.x, height, pos.position.y);
            cube.transform.localScale = (Vector3.one);
        }
    }

}
