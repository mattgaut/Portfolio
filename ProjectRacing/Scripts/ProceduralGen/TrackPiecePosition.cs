using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiecePosition : MonoBehaviour {

    public enum Direction { north = 0, east = 1, south = 2, west = 3 }


    [SerializeField]
    [Tooltip("Relative to Start Direction")]
    Direction relative_end_direction;
    public Direction end_direction {
        get { return (Direction)(((int)relative_end_direction + (int)start_direction) % 4); }
    }
    [SerializeField]
    TrackPosition _end_position;
    public TrackPosition end_position {
        get { return _end_position + position; }
        set { _end_position = value; }
    }

    [SerializeField]
    List<TrackPosition> relative_tiles_occupied;

    public Vector2 position {
        get; private set;
    }
    public Direction start_direction {
        get; private set;
    }

    public int tileCount {
        get { return relative_tiles_occupied.Count; }
    }

    public List<TrackPosition> TilesOccupied() {
        List<TrackPosition> ret = new List<TrackPosition>();
        foreach (TrackPosition tp in relative_tiles_occupied) {
            ret.Add(new TrackPosition(tp.position + position, tp.GetHeight()));
        }
        return ret;
    }
    public List<TrackPosition> TilesOccupied(TrackPiecePosition relative_to) {
        return TilesOccupied(relative_to.end_position, relative_to.end_direction);
    }
    public List<TrackPosition> TilesOccupied(TrackPosition relative_to_position, Direction relative_to_direction) {
        List<TrackPosition> ret = new List<TrackPosition>();
        foreach (TrackPosition tp in relative_tiles_occupied) {
            ret.Add(new TrackPosition((Vector2)(Quaternion.Euler(0, 0, (360 - (int)relative_to_direction * 90)) * tp.position) + NextStartingPoint(relative_to_direction, relative_to_position.position), tp.GetHeight()));
        }
        return ret;
    }

    public void Init(TrackPosition start, Direction direction, float track_scale) {
        position = start.position;

        start_direction = direction;

        List<TrackPosition> new_track_position = new List<TrackPosition>();

        int angle = 360 - 90 * (int)direction;
        if (direction != Direction.north) {

            foreach (TrackPosition tp in relative_tiles_occupied) {
                tp.RotateAroundOrigin(angle);
            }
            _end_position.RotateAroundOrigin(angle);
        }

        transform.position = new Vector3(start.position.x * track_scale, 0, start.position.y * track_scale);
        transform.localRotation = Quaternion.Euler(0, angle * -1, 0);
        transform.localScale = new Vector3(track_scale, track_scale, track_scale);

    }

    public void AttachTo(TrackPiecePosition piece, float track_scale) {
        Init(piece.NextStartingPoint(), piece.end_direction, track_scale);
    }

    public TrackPosition NextStartingPoint() {
        Vector2 pos = new Vector2(0, 0);

        if (end_direction == Direction.north) {
            pos = new Vector2(0, 1);
        } else if (end_direction == Direction.east) {
            pos = new Vector2(1, 0);
        } else if (end_direction == Direction.west) {
            pos = new Vector2(-1, 0);
        } else if (end_direction == Direction.south) {
            pos = new Vector2(0, -1);
        }
        return end_position + pos;
    }
    public static Vector2 NextStartingPoint(Direction end_direction, Vector2 end_position) {
        Vector2 pos = new Vector2(0, 0);

        if (end_direction == Direction.north) {
            pos = new Vector2(0, 1);
        } else if (end_direction == Direction.east) {
            pos = new Vector2(1, 0);
        } else if (end_direction == Direction.west) {
            pos = new Vector2(-1, 0);
        } else if (end_direction == Direction.south) {
            pos = new Vector2(0, -1);
        }
        return end_position + pos;
    }

    public void ForceSetRelativeTiles(List<TrackPosition> set_to) {
        relative_tiles_occupied = set_to;
    }
}

[System.Serializable]
public class TrackPosition : IEquatable<TrackPosition> {

    public enum Height { middle, under, over };
    [SerializeField]
    Vector2 _position;
    public Vector2 position {
        get { return _position; }
        private set {
            _position = value;
        }
    }
    [SerializeField]
    Height height;

    public TrackPosition(int x, int y, Height h) {
        position = new Vector2(x, y);
        height = h;
    }
    public TrackPosition(Vector2 vec, Height h) {
        position = new Vector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        height = h;
    }

    public Vector2 GetPosition() {
        return position;
    }
    public Height GetHeight() {
        return height;
    }
    public void RotateAroundOrigin(int angle) {
        position = Quaternion.Euler(0, 0, angle) * position;
        position.Set(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }
    public TrackPosition ReturnRotateAroundOrigin(int angle) {
        TrackPosition p = new TrackPosition((int)position.x, (int)position.y, height);
        p.RotateAroundOrigin(angle);
        return p;
    }

    public override bool Equals(object obj) {
        return Equals(obj as TrackPosition);
    }
    public bool Equals(TrackPosition other) {
        return other != null && position.x == other.position.x && position.y == other.position.y && other.height == height;
    }
    public override int GetHashCode() {
        return position.GetHashCode() + (int)height;
    }
    public static TrackPosition operator +(TrackPosition lhs, Vector2 rhs) {
        return new TrackPosition(rhs + lhs.position, lhs.height);
    }
    public static TrackPosition operator +(Vector2 lhs, TrackPosition rhs) {
        return rhs + lhs;
    }

}
