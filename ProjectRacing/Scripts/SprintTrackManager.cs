using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintTrackManager : TrackManager {

    [SerializeField]
    Material final_piece_material;

    [SerializeField]
    GameObject finish_line;

    public override void AddPiece(TrackPiece p) {
        if (track.Count < track_count) {
            p.gameObject.SetActive(true);
            track.Add(p);
            track_ids.Add(p, track_ids.Count);
            if (track.Count == 1) {
                foreach (Player player in GameManager.instance.GetPlayers()) {
                    last_visited_piece[player] = p;
                }
            }
            if (track.Count == track_count) {
                GameObject new_finish_line = Instantiate(finish_line);
                Vector2 end_position = (p.track_position.end_position.position + p.track_position.NextStartingPoint().position) / 2 * p.transform.localScale.x;
                new_finish_line.transform.position = new Vector3(end_position.x, 0, end_position.y);

                new_finish_line.transform.SetParent(p.transform, true);

                new_finish_line.transform.rotation = Quaternion.Euler(new Vector3(0, (int)p.track_position.end_direction * 90, 0));
                new_finish_line.transform.localScale *= p.transform.localScale.x;

                p.SetColor(final_piece_material);
            } else {
                p.SetColor(track_material);
            }
        }
    }
    public override void CheckVisit(TrackPiece piece, Player triggering_player) {
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
            GeneralUIController.instance.player_positions.SetPosition(triggering_player.player_number, track.IndexOf(last_visited_piece[triggering_player]) / (float)track_count);
        }
    }
}

