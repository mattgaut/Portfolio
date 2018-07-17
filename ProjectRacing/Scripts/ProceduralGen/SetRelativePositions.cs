using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRelativePositions : MonoBehaviour {

    Dictionary<TrackPosition, bool> checked_positions;
    List<TrackPosition> positions_to_check;

    [SerializeField]
    TrackPiecePosition piece_to_check;

    BoxCollider checker;

    bool collision_detected;

    public void Start() {
        checked_positions = new Dictionary<TrackPosition, bool>();
        positions_to_check = new List<TrackPosition>();
        positions_to_check.Add(new TrackPosition(0,0, TrackPosition.Height.middle));

        piece_to_check.transform.position = transform.position -  new Vector3(0, 0.49f, 0);

        checker = GetComponent<BoxCollider>();
        checker.enabled = false;

        StartCoroutine(Check());
    }

    IEnumerator Check() {
        while (positions_to_check.Count > 0) {
            TrackPosition to_check = positions_to_check[0];
            positions_to_check.RemoveAt(0);

            if (!checked_positions.ContainsKey(to_check)) {
                collision_detected = false;
                yield return CheckPosition(to_check);
                checked_positions.Add(to_check, collision_detected);

                if (collision_detected) {
                    positions_to_check.AddRange(AdjacentPositions(to_check));
                }

                GameObject new_square = GameObject.CreatePrimitive(PrimitiveType.Cube);
                new_square.transform.position = checker.center;
                new_square.transform.localScale = checker.size;

                if (collision_detected) {
                    new_square.GetComponent<MeshRenderer>().material.color = new Color(0,1,0,0.5f);
                } else {
                    new_square.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.5f);
                }

                new_square.GetComponent<Collider>().enabled = false;
            }
        }

        List<TrackPosition> positions = new List<TrackPosition>();
        foreach (TrackPosition pos in checked_positions.Keys) {
            if (checked_positions[pos]) {
                positions.Add(pos);
            }
        }

        piece_to_check.ForceSetRelativeTiles(positions);
    }

    List<TrackPosition> AdjacentPositions(TrackPosition pos) {
        List<TrackPosition> to_ret = new List<TrackPosition>();

        to_ret.Add(pos + new Vector2(0, -1));
        to_ret.Add(pos + new Vector2(0, 1));
        to_ret.Add(pos + new Vector2(-1, 0));
        to_ret.Add(pos + new Vector2(1, 0));

        if (pos.GetHeight() == TrackPosition.Height.middle) {
            to_ret.Add(new TrackPosition(pos.position, TrackPosition.Height.over));
            to_ret.Add(new TrackPosition(pos.position, TrackPosition.Height.under));
        } else {
            to_ret.Add(new TrackPosition(pos.position, TrackPosition.Height.middle));
        }

        return to_ret;
    }

    void SetColliderPosition(TrackPosition tp) {
        if (tp.GetHeight() == TrackPosition.Height.middle) {
            checker.center = new Vector3(tp.position.x, 0, tp.position.y);
            checker.size = Vector3.one * .999f;
        } else {
            checker.center = new Vector3(tp.position.x, tp.GetHeight() == TrackPosition.Height.over ? 6.0f : -6.0f, tp.position.y);
            checker.size = new Vector3(.999f, 10.999f, .999f);
        }
    }

    IEnumerator CheckPosition(TrackPosition p) {
        checker.enabled = true;
        Nudge();

        SetColliderPosition(p);
        float time = 0;
        while (time < 0.15f && !collision_detected) {
            yield return null;
            time += Time.deltaTime;
        }
        checker.enabled = false;
    }
    IEnumerator Nudge() {
        transform.position += new Vector3(0, 0.01f, 0);
        yield return null;
        transform.position -= new Vector3(0, 0.01f, 0);
    }

    void OnTriggerStay(Collider c) {
        collision_detected = true;
    }
}
