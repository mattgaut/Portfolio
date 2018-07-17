using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOneRoom : LevelGenerator {


    public override Dictionary<Vector2Int, Room> Generate() {
        Clear();
        InsertRoom(origin, Vector2Int.zero);

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
}
