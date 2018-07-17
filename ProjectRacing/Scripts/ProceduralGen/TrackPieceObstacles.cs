using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackPiece))]
public class TrackPieceObstacles : MonoBehaviour {

    [SerializeField]
    List<ObstacleSet> obstacles;

    float max_rarity;

    public void SpawnRandomSet(float weight) {
        if (obstacles.Count > 0) {

            if (max_rarity == 0) {
                max_rarity = 0;
                foreach (ObstacleSet ob in obstacles) {
                    max_rarity += ob.rarity;
                }
            }

            ObstacleSet selected = null;
            float rand = Random.Range(0, max_rarity);
            int count = -1;
            while (rand > 0) {
                count++;
                rand -= obstacles[count].rarity;
                selected = obstacles[count];
            }

            GameObject new_obstacles = Instantiate(selected.obstacles);

            new_obstacles.transform.SetParent(transform, false);
        }
    }
}

[System.Serializable]
public class ObstacleSet {
    [SerializeField][Tooltip("Larger is more common")]
    float _rarity;
    public float rarity {
        get { return _rarity; }
    }
    [SerializeField]
    GameObject _obstacles;
    public GameObject obstacles {
        get { return _obstacles; }
    }
}
