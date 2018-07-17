using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : LevelGenerator {

    List<Vector2Int> shape_blocks;
    [SerializeField] [Range(0, 1)] float fill;
    [SerializeField] [Range(3, 20)] int blocks_x, blocks_y;

    public override Dictionary<Vector2Int, Room> Generate() {
        Clear();
        GenerateShapes();
        available_spaces = new HashSet<Vector2Int>(shape_blocks);
        InsertRoom(origin, Vector2Int.zero);


        while ((float)available_spaces.Count/shape_blocks.Count > (1 - fill)) {
            Vector2Int next_space;
            Room r;
            List<Room> remaining_rooms = new List<Room>(possible_rooms);
            Vector2Int offset = Vector2Int.zero;

            bool fit = false;
            do {
                r = remaining_rooms[Random.Range(0, remaining_rooms.Count)];
                remaining_rooms.Remove(r);
                List<Vector2Int> possible_spaces = new List<Vector2Int>(adjacent_spaces);
                do {
                    next_space = possible_spaces[Random.Range(0, possible_spaces.Count)];
                    possible_spaces.Remove(next_space);
                    foreach (Vector2Int i in r.template.GetCoordinateList().Shuffle()) {
                        if (available_spaces.Contains(next_space + i) && RoomCanFit(r, next_space + i)) {
                            fit = true;
                            offset = i;
                            break;
                        }
                    }

                } while (!fit && possible_spaces.Count > 0);
            } while (!fit && remaining_rooms.Count > 0);
            if (!fit) {
                break;
            } else 
                InsertRoom(r, next_space + offset);
        }

        bool boss_room_placed = false;
        while (!boss_room_placed) {
            Vector2Int next_space;
            List<Room> remaining_rooms = new List<Room>(possible_rooms);
            Vector2Int offset = Vector2Int.zero;

            List<Vector2Int> possible_spaces = new List<Vector2Int>(adjacent_spaces);
            do {
                next_space = possible_spaces[Random.Range(0, possible_spaces.Count)];
                possible_spaces.Remove(next_space);
                foreach (Vector2Int i in boss_room.template.GetCoordinateList().Shuffle()) {
                    if (RoomCanFit(boss_room, next_space + i)) {
                        boss_room_placed = true;
                        offset = i;
                        break;
                    }
                }
            } while (!boss_room_placed && possible_spaces.Count > 0);
            if (!boss_room_placed) {
                break;
            } else
                InsertRoom(boss_room, next_space + offset);
        }

        return room_origins;
    }

    void GenerateShapes() {
        shape_blocks = new List<Vector2Int>();

        for (int i = blocks_x; i >= 0; i--) {
            for (int j = blocks_y; j >=0; j--) {
                shape_blocks.Add(new Vector2Int(i, j));
            }
        }
    }
}
