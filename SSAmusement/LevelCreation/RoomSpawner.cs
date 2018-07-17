using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    [SerializeField] TileSet ts;
    [SerializeField] float room_width, room_height;
    [SerializeField] [Range(0, 1)] float mobility;
    [SerializeField] List<Item> item_pool;
    Dictionary<Vector2Int, Room> positions_to_rooms;
    Dictionary<Block, List<Block>> possible_neighbors;
    Dictionary<Room, List<Room>> adjacent_rooms;
    List<Room> rooms;
    BossRoom boss_room;

    private void Awake() {
        positions_to_rooms = new Dictionary<Vector2Int, Room>();
        adjacent_rooms = new Dictionary<Room, List<Room>>();
        possible_neighbors = new Dictionary<Block, List<Block>>();

        rooms = new List<Room>();
    }

    public void Generate(Dictionary<Vector2Int, Room> room_dict) {
        SpawnRooms(room_dict);
        foreach (Vector2Int v in positions_to_rooms.Keys) {
            FindPossibleNeighbors(v);
        }
        foreach (Block b in possible_neighbors.Keys) {
            foreach (Block n in possible_neighbors[b]) {
                if (n.grid_position.y == b.grid_position.y - 1) {
                    b.EnablePathway(Pathway.BOTTOM);
                    adjacent_rooms[b.room].Add(n.room);
                } else if (n.grid_position.y == b.grid_position.y + 1) {
                    b.EnablePathway(Pathway.TOP);
                    adjacent_rooms[b.room].Add(n.room);
                } else if (n.grid_position.x == b.grid_position.x - 1) {
                    b.EnablePathway(Pathway.LEFT);
                    adjacent_rooms[b.room].Add(n.room);
                } else if (n.grid_position.x == b.grid_position.x + 1) {
                    b.EnablePathway(Pathway.RIGHT);
                    adjacent_rooms[b.room].Add(n.room);
                }
            }
        }
    }

    void SpawnRooms(Dictionary<Vector2Int, Room> room_dict) {
        positions_to_rooms = new Dictionary<Vector2Int, Room>();
        foreach (Vector2Int v in room_dict.Keys) {
            Room new_room = Instantiate(room_dict[v], new Vector3(v.x * room_width, v.y * room_height, 0), Quaternion.identity);
            new_room.grid_position = v;
            new_room.SetSize(room_width, room_height);
            adjacent_rooms.Add(new_room, new List<Room>());
            foreach (Vector2Int pos in new_room.template.GetCoordinateList()) {
                positions_to_rooms.Add(pos + v, new_room);
                new_room.template.GetBlock(pos).DisablePathway();
            }
            new_room.LoadTileSet(ts);
            new_room.SpawnRandomRoomset();
            if (new_room as BossRoom != null) {
                boss_room = new_room as BossRoom;
            } else {
                rooms.Add(new_room);
            }
        }

        item_pool.Shuffle();
        List<Room> can_spawn_item = new List<Room>(rooms);
        int target_items = 3;
        while (can_spawn_item.Count > 0 && target_items > 0) {
            can_spawn_item.Shuffle();
            ItemSpawner item_spawn = can_spawn_item[0].GetComponentInChildren<ItemSpawner>();
            if (item_spawn == null) {
                can_spawn_item.RemoveAt(0);
            } else {
                item_spawn.SpawnItemChest().SetSpawnItem(item_pool[0]);
                item_pool.RemoveAt(0);
                target_items--;
                can_spawn_item.RemoveAt(0);
            }
        }

        if (boss_room.reward) {
            boss_room.reward.SpawnItemChest().SetSpawnItem(item_pool[0]);
            item_pool.RemoveAt(0);
        }
        foreach (Room r in rooms) {
            foreach (ItemSpawner ispawn in r.GetComponentsInChildren<ItemSpawner>()) {
                Destroy(ispawn.gameObject);
            }
        }
    }

    void FindPossibleNeighbors(Vector2Int v) {
        Block b = GetBlock(v);
        possible_neighbors.Add(b, new List<Block>());
        if (b.bottom_connection && positions_to_rooms.ContainsKey(v + Vector2Int.down)) {
            Block neighbor = GetBlock(v + Vector2Int.down);
            if (neighbor.top_connection) {
                possible_neighbors[b].Add(neighbor);
            }
        }
        if (b.top_connection && positions_to_rooms.ContainsKey(v + Vector2Int.up)) {
            Block neighbor = GetBlock(v + Vector2Int.up);
            if (neighbor.bottom_connection) {
                possible_neighbors[b].Add(neighbor);
            }
        }
        if (b.left_connection && positions_to_rooms.ContainsKey(v + Vector2Int.left)) {
            Block neighbor = GetBlock(v + Vector2Int.left);
            if (neighbor.right_connection) {
                possible_neighbors[b].Add(neighbor);
            }
        }
        if (b.right_connection && positions_to_rooms.ContainsKey(v + Vector2Int.right)) {
            Block neighbor = GetBlock(v + Vector2Int.right);
            if (neighbor.left_connection) {
                possible_neighbors[b].Add(neighbor);
            }
        }
    }

    Block GetBlock(Vector2Int v) {
        return positions_to_rooms[v].template.GetBlock(v - positions_to_rooms[v].grid_position);
    }

    public Dictionary<Room, List<Room>> GetNeighbors() {
        return adjacent_rooms;
    }

    public Room GetOrigin() {
        return positions_to_rooms[Vector2Int.zero];
    }
}
