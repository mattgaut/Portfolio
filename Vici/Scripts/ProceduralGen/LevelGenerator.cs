using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public Dictionary<IntTuple, GenRoom> generated_rooms;

    [SerializeField]
    int seed;
    [SerializeField]
    bool use_random_seed;


    [SerializeField]
    int room_width, room_height;
    [SerializeField]
    GameObject room, boss_room, item_room;
    [SerializeField]
    List<GameObject> regular_layouts;
    [SerializeField]
    List<GameObject> possible_items;
    [SerializeField]
    GameObject spawn_layout;
    [SerializeField]
    List<GameObject> boss_layouts;
    [SerializeField]
    GameObject item_layout;
    public Dictionary<IntTuple, Room> spawned_rooms;

    [SerializeField]
    List<PickUpDrop> drops;
    [SerializeField]
    GameObject boss_key;

	// Use this for initialization
	void Start () {
        generated_rooms = new Dictionary<IntTuple, GenRoom>();
        spawned_rooms = new Dictionary<IntTuple, Room>();
	}

    public void GenerateLevel() {
        if (!use_random_seed) {
            Random.InitState(seed);
        } else {
            Random.InitState(System.DateTime.Now.Millisecond);
        }

        generated_rooms.Clear();
        generated_rooms.Add(new IntTuple(0,0), new GenRoom());

        int chains = Random.Range(3, 7);
        for (int i = 0; i < chains; i++) {
            int chain_length = 0;
            IntTuple previous = new IntTuple(0, 0);
            IntTuple current;

            float rand = Random.Range(0, 1f);
            if (rand > .75f) {
                current = new IntTuple(0, 1);
            } else if (rand > .5f) {
                current = new IntTuple(0, -1);
            } else if (rand > .25f) {
                current = new IntTuple(1, 0);
            } else {
                current = new IntTuple(-1, 0);
            }

            while (Random.Range(0, 1f) < (1.1f - chain_length/10f) && generated_rooms.Count < 11) {
                if (!generated_rooms.ContainsKey(current)) {
                    generated_rooms.Add(current, new GenRoom());
                }

                rand = Random.Range(0, 1f);
                if (rand > 2f/3f) {
                    int x = current.x - previous.x;
                    int y = current.y - previous.y;

                    previous = current;
                    current = new IntTuple(x + current.x, y + current.y);
                } else if (rand > 1f/3f){
                    int x = current.x - previous.x;
                    int y = current.y - previous.y;

                    previous = current;
                    current = new IntTuple(y + current.x, x + current.y);
                } else {
                    int x = current.x - previous.x;
                    int y = current.y - previous.y;

                    previous = current;
                    current = new IntTuple(-y + current.x, -x + current.y);
                }
                chain_length += 1;
            }
        }
        // Set Spawn Room
        generated_rooms[new IntTuple(0, 0)].type = RoomType.spawn;
        // Set Boss Room
        bool set = false;
        foreach (IntTuple tup in generated_rooms.Keys) {
            if (HasOneEntrance(tup) && generated_rooms[tup].type == RoomType.normal) {
                set = true;
                generated_rooms[tup].type = RoomType.boss;
                break;
            }
        }
        if (!set) {
            IntTuple tup = FindCreateOneEntranceRoom();
            generated_rooms[tup].type = RoomType.boss;
        }
        // Set Item Room
        set = false;
        foreach (IntTuple tup in generated_rooms.Keys) {
            if (HasOneEntrance(tup) && generated_rooms[tup].type == RoomType.normal) {
                set = true;
                generated_rooms[tup].type = RoomType.item;
                break;
            }
        }
        if (!set) {
            IntTuple tup = FindCreateOneEntranceRoom();
            generated_rooms[tup].type = RoomType.item;
        }

        SpawnRooms();
    }

    IntTuple FindCreateOneEntranceRoom() {
        foreach (IntTuple tup in generated_rooms.Keys) {
            if (generated_rooms[tup].type != RoomType.normal) continue;
            if (HasOneEntrance(tup)) {
                return tup;
            }
        }
        List<IntTuple> tups = new List<IntTuple>(generated_rooms.Keys);
        for (int i = 0; i < generated_rooms.Keys.Count; i++) {
            IntTuple tup = tups[i];
            IntTuple neighbor = new IntTuple(tup.x + 1, tup.y);
            if (!generated_rooms.ContainsKey(neighbor) && HasOneEntrance(neighbor)) {
                generated_rooms.Add(neighbor, new GenRoom());
                return neighbor;
            }
            neighbor = new IntTuple(tup.x - 1, tup.y);
            if (!generated_rooms.ContainsKey(neighbor) && HasOneEntrance(neighbor)) {
                generated_rooms.Add(neighbor, new GenRoom());
                return neighbor;
            }
            neighbor = new IntTuple(tup.x, tup.y - 1);
            if (!generated_rooms.ContainsKey(neighbor) && HasOneEntrance(neighbor)) {
                generated_rooms.Add(neighbor, new GenRoom());
                return neighbor;
            }
            neighbor = new IntTuple(tup.x, tup.y + 1);
            if (!generated_rooms.ContainsKey(neighbor) && HasOneEntrance(neighbor)) {
                generated_rooms.Add(neighbor, new GenRoom());
                return neighbor;
            }
        }
        return null;
    }
    bool HasOneEntrance(IntTuple tup) {
        int count = 0;
        if (generated_rooms.ContainsKey(new IntTuple(tup.x + 1, tup.y))) {
            count++;
        }
        if (generated_rooms.ContainsKey(new IntTuple(tup.x, tup.y + 1))) {
            count++;
        }
        if (generated_rooms.ContainsKey(new IntTuple(tup.x - 1, tup.y))) {
            count++;
        }
        if (generated_rooms.ContainsKey(new IntTuple(tup.x, tup.y - 1))) {
            count++;
        }
        return count == 1;
    }

    void SpawnRooms() {
        foreach (IntTuple tup in generated_rooms.Keys) {
            GameObject new_room;
            if (generated_rooms[tup].type == RoomType.boss) {
                new_room = Instantiate(boss_room, new Vector3(tup.x * room_width, tup.y * room_height, 0), Quaternion.identity);
            } else if (generated_rooms[tup].type == RoomType.item) {
                new_room = Instantiate(item_room, new Vector3(tup.x * room_width, tup.y * room_height, 0), Quaternion.identity);
            } else {
                new_room = Instantiate(room, new Vector3(tup.x * room_width, tup.y * room_height, 0), Quaternion.identity);
            }
            spawned_rooms.Add(tup, new_room.GetComponent<Room>());
            spawned_rooms[tup].SetPosition(tup);
        }
        foreach (IntTuple tup in spawned_rooms.Keys) {
            IntTuple n_tup = new IntTuple(tup.x - 1, tup.y);
            if (spawned_rooms.ContainsKey(n_tup)) {
                spawned_rooms[n_tup].SetRightNeighbor(spawned_rooms[tup]);
            }
            n_tup = new IntTuple(tup.x + 1, tup.y);
            if (spawned_rooms.ContainsKey(n_tup)) {
                spawned_rooms[n_tup].SetLeftNeighbor(spawned_rooms[tup]);
            }
            n_tup = new IntTuple(tup.x, tup.y + 1);
            if (spawned_rooms.ContainsKey(n_tup)) {
                spawned_rooms[n_tup].SetBottomNeighbor(spawned_rooms[tup]);
            }
            n_tup = new IntTuple(tup.x, tup.y - 1);
            if (spawned_rooms.ContainsKey(n_tup)) {
                spawned_rooms[n_tup].SetTopNeighbor(spawned_rooms[tup]);
            }
        }
        foreach (Room r in spawned_rooms.Values) {
            r.CreateDoorsAndWalls();
            GameObject layout;
            do {
                layout = regular_layouts[Random.Range(0, regular_layouts.Count)];
            } while (!LayoutFitsRoom(layout.GetComponent<RoomLayout>(), r));
            SetLayout(r);
            r.SetRoomActive(false);
        }

        foreach (Room r in spawned_rooms.Values) {
            foreach (Enemy e in r.room_layout.enemies_in_room) {
                foreach (PickUpDrop pud in drops) {
                    if (Random.Range(0f, 1f) < pud.chance_per_enemy) {
                        e.AddPickup(pud.pickup);
                    }
                }
            }
        }

        RoomLayout boss_key_room;
        List<Room> poss_rooms = new List<Room>(spawned_rooms.Values);
        do {
            boss_key_room = poss_rooms[Random.Range(0, poss_rooms.Count - 1)].room_layout;
        } while (boss_key_room.room_type != RoomType.normal);

        boss_key_room.AddDrop(boss_key);

        LevelManager.instance.SetInitialActive(spawned_rooms[new IntTuple(0,0)]);
    }

    void SetLayout(Room r) {
        if (generated_rooms[r.position].type == RoomType.normal) {
            RoomLayout layout;
            do {
                layout = regular_layouts[Random.Range(0, regular_layouts.Count)].GetComponent<RoomLayout>();
            } while (!((r.left_neighbor == null || layout.left_neighbor) && (r.right_neighbor == null || layout.right_neighbor) && (r.top_neighbor == null || layout.top_neighbor) && (r.bottom_neighbor == null || layout.bottom_neighbor)));
            r.SetLayout(layout.gameObject);
        }
        if (generated_rooms[r.position].type == RoomType.spawn) {
            r.SetLayout(spawn_layout);
        }
        if (generated_rooms[r.position].type == RoomType.boss) {
            GameObject new_layout = Instantiate(boss_layouts[Random.Range(0, boss_layouts.Count)], r.transform, false);
            new_layout.GetComponent<BossRoomLayout>().SetItem(possible_items[Random.Range(0, possible_items.Count)]);
            r.SetLayout(new_layout.GetComponent<BossRoomLayout>());
        }
        if (generated_rooms[r.position].type == RoomType.item) {
            GameObject new_layout = Instantiate(item_layout, r.transform, false);
            new_layout.GetComponent<ItemRoomLayout>().SetItem(possible_items[Random.Range(0, possible_items.Count)]);
            r.SetLayout(new_layout.GetComponent<ItemRoomLayout>());
        }
    }

    bool LayoutFitsRoom(RoomLayout layout, Room room) {
        if (room.left_neighbor != null && layout.left_neighbor == false) {
            return false;
        }
        if (room.right_neighbor != null && layout.right_neighbor == false) {
            return false;
        }
        if (room.top_neighbor != null && layout.top_neighbor == false) {
            return false;
        }
        if (room.bottom_neighbor != null && layout.bottom_neighbor == false) {
            return false;
        }
        return true;
    }
}

public class GenRoom {
    public RoomType type;

    public GenRoom(RoomType t = RoomType.normal) {
        type = t;
    }
}

[System.Serializable]
public class PickUpDrop {
    [SerializeField]
    GameObject _pickup;
    [SerializeField]
    float _chance_per_enemy;

    public GameObject pickup {
        get { return _pickup; }
    }
    public float chance_per_enemy {
        get { return _chance_per_enemy; }
    } 
}

public class IntTuple : System.IEquatable<IntTuple> {
    public int x {
        get; private set;
    }
    public int y {
        get; private set;
    }

    public IntTuple(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public bool Equals(IntTuple other) {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType())
            return false;
        if (ReferenceEquals(obj, this))
            return true;
        IntTuple other = (IntTuple)obj;
        return x == other.x && y == other.y;
    }

    public override int GetHashCode() {
        return x.GetHashCode() + y.GetHashCode();
    }
}
