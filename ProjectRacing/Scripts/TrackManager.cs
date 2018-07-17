using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour {

    public static TrackManager instance {
        get; private set;
    }

    [SerializeField]
    TrackGenerator _generator;
    public TrackGenerator generator {
        get { return _generator; }
    }

    protected List<TrackPiece> track;
    public TrackPiece latest_piece {
        get { return track[track.Count - 1]; }
    }

    [SerializeField]
    protected int track_count, leading_track_count;

    [SerializeField]
    bool auto_grab_pieces;

    [SerializeField]
    protected Material track_material;

    protected Dictionary<Player, TrackPiece> last_visited_piece;
    protected Dictionary<TrackPiece, int> track_ids;

    int pieces_removed;

    public void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void Start() {
        track = new List<TrackPiece>();

        StartCoroutine(generator.GenerateTrackPieces());

        if (auto_grab_pieces)
            StartCoroutine(GrabPiece(0.25f));
        else
            StartCoroutine(GrabFirstPieces(0.5f));

        last_visited_piece = new Dictionary<Player, TrackPiece>();
        track_ids = new Dictionary<TrackPiece, int>();

        foreach (Player p in GameManager.instance.GetPlayers()) {
            last_visited_piece.Add(p, null);
        }
    }

    public virtual void AddPiece(TrackPiece p) {
        p.gameObject.SetActive(true);

        track.Add(p);
        track_ids.Add(p, pieces_removed + track_ids.Count);
        if (track.Count == 1) {
            foreach (Player player in GameManager.instance.GetPlayers()) {
                last_visited_piece[player] = p;
            }
        }

        if (track.Count > track_count) {
            RemoveOldestPiece();
        }

        p.SetColor(track_material);
    }
    public void AddNextPiece() {
        AddPiece(generator.GetNextPiece());
    }
    public virtual void CheckVisit(TrackPiece piece, Player triggering_player) {
        int index = track.IndexOf(piece);
        if (triggering_player.is_active) {
            if (index >= 0) {
                int distance_from_end = track.Count - index - 1;
                while (distance_from_end < leading_track_count) {
                    AddNextPiece();
                    distance_from_end++;
                }
                GameManager.instance.RespawnPlayers(piece);
            } else {
                while (track[track.Count - 1] != piece) {
                    AddNextPiece();
                }
                CheckVisit(piece, triggering_player);
                return;
            }
        }

        last_visited_piece[triggering_player] = piece;
        if (GeneralUIController.instance != null) {
            GeneralUIController.instance.player_positions.SetPosition(triggering_player.player_number, track.IndexOf(last_visited_piece[triggering_player]) / ((float)track_count));
        }
    }
    public int GetTrackPiecesPassed(Player p) {
        return track_ids[last_visited_piece[p]];
    }
    public int GetPositionOnTrack(Player p) {
        return track.IndexOf(last_visited_piece[p]);
    }
    public int DistanceBetweenFirstAndLast() {
        int max = -1, min = 99999;
        foreach (int i in track_ids.Values) {
            if (i < min) {
                min = i;
            } 
            if (i > max) {
                max = i;
            }
        }
        return max - min;
    }
    public int DistanceFromLast(Player p) {
        int min = 99999;
        foreach (int i in track_ids.Values) {
            if (i < min) {
                min = i;
            }
        }
        return track_ids[last_visited_piece[p]] - min;
    }

    public void RemoveOldestPiece() {
        generator.RemoveFromMap(track[0]);
        Destroy(track[0].gameObject);
        track.RemoveAt(0);
        pieces_removed++;

        if (GeneralUIController.instance != null) {
            foreach (Player p in GameManager.instance.GetPlayers()) {
                GeneralUIController.instance.player_positions.SetPosition(p.player_number, track.IndexOf(last_visited_piece[p]) / (float)(track_count));
            }
        }
    }

    IEnumerator GrabPiece(float time_between_pieces) {
        float time = 0;
        while (true) {
            yield return null;
            time += Time.deltaTime;
            if (time > time_between_pieces) {
                time -= time_between_pieces;
                AddPiece(generator.GetNextPiece());
            }
        }
    }
    IEnumerator GrabFirstPieces(float after_time) {
        float time = 0;
        while (time < after_time) {
            yield return null;
            time += Time.deltaTime;
        }
        time = 0;
        while (track.Count < leading_track_count) {
            yield return null;
            AddPiece(generator.GetNextPiece());
        }
    }

    public TrackPiece FirstPiece() {
        return track[0];
    }
    public int TrackId(TrackPiece piece) {
        return track_ids.ContainsKey(piece) ? track_ids[piece] : -1;
    }
    public TrackPiece LastPiece() {
        return track[track.Count - 1];
    }
    public TrackPiece LastPiece(Player p) {
        return last_visited_piece.ContainsKey(p) ? last_visited_piece[p] : null;
    }
}