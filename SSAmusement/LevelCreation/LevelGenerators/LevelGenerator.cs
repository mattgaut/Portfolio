using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGenerator : MonoBehaviour {

    protected Dictionary<Vector2Int, Block> tiles;
    protected Dictionary<Vector2Int, Room> room_origins;
    protected HashSet<Vector2Int> available_spaces;
    [SerializeField] protected Room origin;
    [SerializeField] protected List<Room> possible_rooms;
    [SerializeField] protected Room boss_room;
    protected List<Vector2Int> adjacent_spaces;

    protected virtual void Clear() {
        tiles = new Dictionary<Vector2Int, Block>();
        available_spaces = new HashSet<Vector2Int>();
        room_origins = new Dictionary<Vector2Int, Room>();
        adjacent_spaces = new List<Vector2Int>();
    }

    public abstract Dictionary<Vector2Int, Room> Generate();

    protected virtual void InsertRoom(Room r, Vector2Int position) {
        room_origins.Add(position, r);
        foreach (Vector2Int local_position in r.template.GetCoordinateList()) {
            Block b = r.template.GetBlock(local_position);
            tiles.Add(position + local_position, b);
            if (available_spaces.Contains(position + local_position)) {
                available_spaces.Remove(position + local_position);
            }
            adjacent_spaces.Remove(position);


            Vector2Int pos = local_position + position + Vector2Int.down;
            if (b.bottom_connection && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }
            pos = local_position + position + Vector2Int.up;
            if (b.top_connection && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }
            pos = local_position + position + Vector2Int.left;
            if (b.left_connection && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }
            pos = local_position + position + Vector2Int.right;
            if (b.right_connection && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }
            
        }
    }

    protected bool RoomCanFit(Room r, Vector2Int position) {
        foreach (Vector2Int local_position in r.template.GetCoordinateList()) {
            if (tiles.ContainsKey(position + local_position)) {
                return false;
            }
        }
        foreach (Vector2Int local_position in r.template.GetCoordinateList()) {
            if (BlockConnects(r.template.GetBlock(local_position), position + local_position)) {
                return true;
            }
        }
        return false;
    }

    protected bool BlockConnects(Block b, Vector2Int position) {
        if (b.bottom_connection && tiles.ContainsKey(position + Vector2Int.down) && tiles[position + Vector2Int.down].top_connection) {
            return true;
        }
        if (b.top_connection && tiles.ContainsKey(position + Vector2Int.up) && tiles[position + Vector2Int.up].bottom_connection) {
            return true;
        }
        if (b.left_connection && tiles.ContainsKey(position + Vector2Int.left) && tiles[position + Vector2Int.left].right_connection) {
            return true;
        }
        if (b.right_connection && tiles.ContainsKey(position + Vector2Int.right) && tiles[position + Vector2Int.right].left_connection) {
            return true;
        }
        return false;
    }

    public Room GetInitialRoom() {
        return room_origins[Vector2Int.zero];
    }
}