using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pathway { TOP, BOTTOM, LEFT, RIGHT }

public class Block : MonoBehaviour {
    [SerializeField] Vector2Int _position;
    [SerializeField] bool _left_connection, _right_connection, _top_connection, _bottom_connection;
    [SerializeField] GameObject left_pathway, right_pathway, top_pathway, bottom_pathway;
    [SerializeField] GameObject left_wall, right_wall, top_wall, bottom_wall;
    public Room room { get; private set; }
    
    public Vector2Int local_position { get { return _position; } }
    public Vector2Int grid_position { get { return _position + room.grid_position; } }

    public bool left_connection { get { return _left_connection; } }
    public bool right_connection { get { return _right_connection; } }
    public bool top_connection { get { return _top_connection; } }
    public bool bottom_connection { get { return _bottom_connection; } }

    public bool left_pathway_open { get; private set; }
    public bool right_pathway_open { get; private set; }
    public bool top_pathway_open { get; private set; }
    public bool bottom_pathway_open { get; private set; }

    public Block() { }

    public void SetBlock(Room r, Vector2Int position) {
        room = r;
        _position = position;
    }

    public void SetRoom(Room r) {
        room = r;
    }

    public List<Tile> TrimWall(Pathway p) {
        List<Tile> destroyed = new List<Tile>();
        if (p == Pathway.BOTTOM && bottom_wall != null) {
            Destroy(bottom_wall);
            _bottom_connection = false;
            destroyed.AddRange(bottom_wall.GetComponentsInChildren<Tile>());
        } else if (p == Pathway.LEFT && left_wall != null) {
            Destroy(left_wall);
            _left_connection = false;
            destroyed.AddRange(left_wall.GetComponentsInChildren<Tile>());
        } else if (p == Pathway.RIGHT && right_wall != null) {
            Destroy(right_wall);
            _right_connection = false;
            destroyed.AddRange(right_wall.GetComponentsInChildren<Tile>());
        } else if (p == Pathway.TOP && top_wall != null) {
            Destroy(top_wall);
            _top_connection = false;
            destroyed.AddRange(top_wall.GetComponentsInChildren<Tile>());
        }
        return destroyed;
    }

    public void SetPathwayObject(Pathway p, GameObject set) {
        if (p == Pathway.TOP) {
            top_pathway = set;
        } else if (p == Pathway.RIGHT) {
            right_pathway = set;
        } else if (p == Pathway.BOTTOM) {
            bottom_pathway = set;
        } else if (p == Pathway.LEFT) {
            left_pathway = set;
        }
    }
    public void SetPathway(Pathway p, bool set) {
        if (p == Pathway.TOP) {
            _top_connection = set;
        } else if (p == Pathway.RIGHT) {
            _right_connection = set;
        } else if (p == Pathway.BOTTOM) {
            _bottom_connection = set;
        } else if (p == Pathway.LEFT) {
            _left_connection = set;
        }
    }

    public void DisablePathway(Pathway pathway) {
        if (pathway == Pathway.TOP && top_connection) {
            SetPathwayToWall(top_pathway, true);
        } else if (pathway == Pathway.RIGHT && right_connection) {
            SetPathwayToWall(right_pathway, true);
        } else if (pathway == Pathway.BOTTOM && bottom_connection) {
            SetPathwayToWall(bottom_pathway, true);
        } else if (pathway == Pathway.LEFT && left_connection) {
            SetPathwayToWall(left_pathway, true);
        }
    }
    public void DisablePathway(Vector2Int pathway) {
        if (pathway == Vector2Int.up && top_connection) {
            SetPathwayToWall(top_pathway, true);
        } else if (pathway == Vector2Int.right && right_connection) {
            SetPathwayToWall(right_pathway, true);
        } else if (pathway == Vector2Int.down && bottom_connection) {
            SetPathwayToWall(bottom_pathway, true);
        } else if (pathway == Vector2Int.left && left_connection) {
            SetPathwayToWall(left_pathway, true);
        }
    }
    public void DisablePathway() {
        if (top_pathway) SetPathwayToWall(top_pathway, true);
        if (right_pathway) SetPathwayToWall(right_pathway, true);
        if (left_pathway) SetPathwayToWall(left_pathway, true);
        if (bottom_pathway) SetPathwayToWall(bottom_pathway, true);
    }
    public void EnablePathway(Pathway pathway) {
        if (pathway == Pathway.TOP && top_connection) {
            SetPathwayToWall(top_pathway, false);
        } else if (pathway == Pathway.RIGHT && right_connection) {
            SetPathwayToWall(right_pathway, false);
        } else if (pathway == Pathway.BOTTOM && bottom_connection) {
            SetPathwayToWall(bottom_pathway, false);
        } else if (pathway == Pathway.LEFT && left_connection) {
            SetPathwayToWall(left_pathway, false);
        }
    }
    public void EnablePathway(Vector2Int pathway) {
        if (pathway == Vector2Int.up && top_connection) {
            SetPathwayToWall(top_pathway, false);
        } else if (pathway == Vector2Int.right && right_connection) {
            SetPathwayToWall(right_pathway, false);
        } else if (pathway == Vector2Int.down && bottom_pathway) {
            SetPathwayToWall(bottom_pathway, false);
        } else if (pathway == Vector2Int.left && left_connection) {
            SetPathwayToWall(left_pathway, false);
        }
    }
    public void EnablePathway() {
        if (top_pathway) SetPathwayToWall(top_pathway, false);
        if (right_pathway) SetPathwayToWall(right_pathway, false);
        if (left_pathway) SetPathwayToWall(left_pathway, false);
        if (bottom_pathway) SetPathwayToWall(bottom_pathway, false);
    }
    void SetPathwayToWall(GameObject pathway, bool set) {
        if (pathway == top_pathway) {
            top_pathway_open = !set;
        } else if (pathway == left_pathway) {
            left_pathway_open = !set;
        } else if (pathway == right_pathway) {
            right_pathway_open = !set;
        } else if (pathway == bottom_pathway) {
            bottom_pathway_open = !set;
        }
        foreach (Tile t in pathway.GetComponentsInChildren<Tile>(true)) {
            if (t.tile_type == TileType.Platform) {
                t.gameObject.SetActive(!set);
            } else {
                t.gameObject.SetActive(set);
            }
        }
    }
}
