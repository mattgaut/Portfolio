using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackGenerator : MonoBehaviour {
    [SerializeField]
    List<TrackRarity> track_objects;
    float max_rarity;
    [SerializeField]
    TrackManager manager;
    [SerializeField]
    int lead, track_scale;

    [SerializeField]
    TrackPiece starting_piece;

    [SerializeField]
    int seed;
    [SerializeField]
    bool use_random_seed;

    [SerializeField]
    Material gen_material;


    List<TrackPiece> future_track_pieces;
    Dictionary<TrackPosition, TrackRarityList> other_possible_pieces;
    Dictionary<TrackPosition, TrackPiece> map;

    public void Awake() {
        if (use_random_seed) {
            seed = System.DateTime.Now.Millisecond;
            Random.InitState(seed);
        } else 
            Random.InitState(seed);

        future_track_pieces = new List<TrackPiece>();
        other_possible_pieces = new Dictionary<TrackPosition, TrackRarityList>();
        map = new Dictionary<TrackPosition, TrackPiece>();

        SetFirstPiece(starting_piece);
    }

    public IEnumerator GenerateTrackPieces() {
        while (true) {
            yield return null;
            if (future_track_pieces.Count < lead) {
                TrackPosition gen_at = GetNextGenPosition();
                if (other_possible_pieces.ContainsKey(gen_at)) {
                    List<TrackRarity> possible_pieces = other_possible_pieces[gen_at].pieces;
                    TrackPiece selected = null;
                    if (possible_pieces.Count > 0) {
                        // If possible pieces remain try one
                        float rand = Random.Range(0, other_possible_pieces[gen_at].rarity);
                        int count = -1;

                        float rarity_to_remove = 0;
                        while (rand > 0) {
                            count++;
                            selected = possible_pieces[count].piece;
                            rand -= possible_pieces[count].rarity;
                            rarity_to_remove = possible_pieces[count].rarity;
                        }
                        possible_pieces.RemoveAt(count);
                        other_possible_pieces[gen_at].rarity -= rarity_to_remove;

                        if (PieceCanFit(selected)) {
                            // If can fit add it to future pieces
                            AddPiece(selected);
                        }
                    } else {
                        // If no possible pieces remain backtrack
                        RemoveLastPiece();
                    }
                } else {
                    // if gen at does not exist add it to the gen with all track_objects as possible;
                    other_possible_pieces.Add(gen_at, new TrackRarityList(track_objects));
                }
            }
        }
    }

    void SetFirstPiece(TrackPiece selected) {
        starting_piece.track_position.Init(new TrackPosition(Vector2.zero, TrackPosition.Height.middle), TrackPiecePosition.Direction.north, track_scale);

        selected.transform.SetParent(transform);
        future_track_pieces.Add(selected);

        foreach (TrackPosition tp in selected.track_position.TilesOccupied()) {
            map.Add(tp, selected);
        }

        selected.SetColor(gen_material);
    }

    void AddPiece(TrackPiece p) {
        TrackPiece selected = InstantiateTrackPiece(p);
        selected.transform.SetParent(transform);
        selected.track_position.AttachTo(GetLastTrackPiece().track_position, track_scale);
        future_track_pieces.Add(selected);

        if (other_possible_pieces.ContainsKey(selected.track_position.end_position)) {
            other_possible_pieces.Remove(selected.track_position.end_position);
        }

        other_possible_pieces.Add(selected.track_position.end_position, new TrackRarityList(track_objects));

        foreach (TrackPosition tp in selected.track_position.TilesOccupied()) {
            map.Add(tp, p);
        }

        GenerateObstacles(selected);

        selected.SetColor(gen_material);

        selected.gameObject.SetActive(false);
    }

    void GenerateObstacles(TrackPiece p) {
        p.obstacles.SpawnRandomSet(0);
    }

    TrackPiece RemoveLastPiece() {
        TrackPiece to_return = future_track_pieces[future_track_pieces.Count - 1];

        RemoveFromMap(to_return);

        other_possible_pieces.Remove(to_return.track_position.end_position);
        Destroy(to_return.gameObject);
        future_track_pieces.RemoveAt(future_track_pieces.Count - 1);

        return to_return;
    }

    public TrackPiece GetNextPiece() {
        TrackPiece to_return = future_track_pieces[0];

        other_possible_pieces.Remove(to_return.track_position.end_position);
        future_track_pieces.RemoveAt(0);

        return to_return;
    }

    TrackPiece InstantiateTrackPiece(TrackPiece prefab) {
        return Instantiate(prefab).GetComponent<TrackPiece>();
    }

    TrackPosition GetNextGenPosition() {
        return GetLastTrackPiece().track_position.NextStartingPoint();
    }

    TrackPiece GetLastTrackPiece() {
        if (future_track_pieces.Count > 0) {
            return future_track_pieces[future_track_pieces.Count - 1];
        } else {
            return manager.LastPiece();
        }
    }

    public bool PieceCanFit(TrackPiece p) {
        if (future_track_pieces.Count > 0) {
            foreach (TrackPosition pos in p.track_position.TilesOccupied(future_track_pieces[future_track_pieces.Count - 1].track_position)) {
                if (map.ContainsKey(pos)) {
                    return false;
                }
            }
        }

        return true;
    }

    public void RemoveFromMap(TrackPiece p) {
        foreach (TrackPosition tp in p.track_position.TilesOccupied()) {
            map.Remove(tp);
        }
    }

    public Vector3 TrackPiecePositionToWorldPosition(TrackPiecePosition p) {
        return new Vector3(p.position.x, 0, p.position.y) * track_scale;
    }
}
public class TrackRarityList {
    public List<TrackRarity> pieces;

    public float rarity;

    public TrackRarityList(List<TrackRarity> list) {
        pieces = new List<TrackRarity>(list);
        rarity = 0;
        rarity = list.Select((a) => a.rarity).Aggregate((a, b) => a + b);
    }
}

[System.Serializable]
public class TrackRarity {
    public TrackPiece piece;
    [Tooltip("Larger Number = More Common")]
    public float rarity;
}
