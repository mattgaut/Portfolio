using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobGenerator : LevelGenerator {


    [SerializeField] int target_size;

    public override Dictionary<Vector2Int, Room> Generate() {
        Clear();
        InsertRoom(origin, Vector2Int.zero);
        while (tiles.Count < target_size) {
            Vector2Int next_space;
            Room r;
            List<Room> remaining_rooms = new List<Room>(possible_rooms);

            bool fit = false;
            Vector2Int offset = Vector2Int.zero;
            do {
                r = remaining_rooms[Random.Range(0, remaining_rooms.Count)];
                remaining_rooms.Remove(r);
                List<Vector2Int> possible_spaces = new List<Vector2Int>(adjacent_spaces);
                do {
                    next_space = possible_spaces[Random.Range(0, possible_spaces.Count)];
                    possible_spaces.Remove(next_space);
                    foreach (Vector2Int i in r.template.GetCoordinateList().Shuffle()) {
                        if (RoomCanFit(r, next_space + i)) {
                            fit = true;
                            offset = i;
                            break;
                        }
                    }
                } while (!fit && possible_spaces.Count > 0);
            } while (!fit);


            InsertRoom(r, next_space + offset);
        }
        return room_origins;
    }
}
