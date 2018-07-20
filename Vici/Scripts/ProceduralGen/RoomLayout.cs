using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { normal, item, boss, spawn };
public class RoomLayout : MonoBehaviour {

    [SerializeField]
    bool _top_neighbor, _bottom_neighbor, _right_neighbor, _left_neighbor;

    [SerializeField]
    float tile_scale;

    [SerializeField]
    RoomType _room_type;
    public RoomType room_type {
        get { return _room_type; }
    }

    public bool top_neighbor {
        get { return _top_neighbor; }
    }
    public bool bottom_neighbor {
        get { return _bottom_neighbor; }
    }
    public bool right_neighbor {
        get { return _right_neighbor; }
    }
    public bool left_neighbor {
        get { return _left_neighbor; }
    }

    public virtual bool can_unlock {
        get { return enemies.Count == 0; }
    }

    [SerializeField]
    protected List<Spawn> spawn_enemies;
    protected Dictionary<Enemy, Vector3> enemies;
    protected List<GameObject> enemy_objects;

    public List<Enemy> enemies_in_room {
        get { return new List<Enemy>(enemies.Keys); }
    }


    [SerializeField]
    protected GameObject spawn_drop_location;

    protected List<GameObject> drops_to_spawn;
    bool objects_spawned = false;

    public int enemies_count {
        get { return enemy_objects.Count; }
    }

    public Pathfinder pathfinding_mesh {
        get { return mesh.pathfinder; }
    }

    [SerializeField]
    bool grab_tiles_automatically;
    [SerializeField]
    GameObject tile_holder;
    [SerializeField]
    protected List<Spawn> spawn_tiles;
    [SerializeField]
    protected PathfindingMesh mesh;

    public Dictionary<IntTuple, Tile> sorted_tiles {
        get; private set;
    }

    protected virtual void Awake() {
        sorted_tiles = new Dictionary<IntTuple, Tile>();
        enemies = new Dictionary<Enemy, Vector3>();
        enemy_objects = new List<GameObject>();
        drops_to_spawn = new List<GameObject>();

        if (grab_tiles_automatically) {
            sorted_tiles = new Dictionary<IntTuple, Tile>();
            foreach (Spawn s in tile_holder.GetComponentsInChildren<Spawn>()) {
                Tile t = s.SpawnObject().GetComponent<Tile>();
                sorted_tiles.Add(new IntTuple((int)(t.transform.localPosition.x / tile_scale), (int)(t.transform.localPosition.y / tile_scale)), t);
            }
        } else {
            foreach (Spawn s in spawn_tiles) {
                Tile t = s.SpawnObject().GetComponent<Tile>();
                sorted_tiles.Add(new IntTuple((int)(t.transform.localPosition.x / tile_scale), (int)(t.transform.localPosition.y / tile_scale)), t);
            }
        }
        foreach (IntTuple tup in sorted_tiles.Keys) {
            sorted_tiles[tup].SetTile(this, tup);
        }
        foreach (Spawn spawn in spawn_enemies) {
            GameObject new_enemy = spawn.SpawnObject();
            enemies.Add(new_enemy.GetComponent<Enemy>(), new_enemy.transform.position);
            enemy_objects.Add(new_enemy);
        }
    }

    protected virtual void Start() {
        foreach (Enemy e in enemies.Keys) {
            e.SetRoom(this);
            if (e.GetComponent<PathfinderAgent>() != null) {
                e.GetComponent<PathfinderAgent>().Initiate();
            }
        }
        DisableRoom();
    }

    public void SetRoomEnabled(bool enabled) {
        if (enabled) {
            EnableRoom();
        } else {
            DisableRoom();
        }
    }

    protected virtual void DisableRoom() {
        foreach (GameObject e in enemy_objects) {
            e.SetActive(false);
        }
    }

    protected virtual void EnableRoom() {
        foreach (GameObject e in enemy_objects) {
            e.SetActive(true);
            e.transform.position = enemies[e.GetComponent<Enemy>()];
        }
        if (!objects_spawned && enemies.Count == 0) {
            SpawnObjects();
        }
    }

    public virtual void RemoveEnemy(Enemy e) {
        enemies.Remove(e);
        enemy_objects.Remove(e.gameObject);

        if (enemies.Count == 0) {
            SpawnObjects();
        }
    }

    protected virtual void SpawnObjects() {
        objects_spawned = true;
        foreach (GameObject go in drops_to_spawn) {
            Instantiate(go, spawn_drop_location ? spawn_drop_location.transform.position : transform.position, Quaternion.identity);
        }
    }

    public void DropInRoom(GameObject drop, Vector3 try_position) {
        try_position -= transform.position;
        IntTuple position = new IntTuple(Mathf.RoundToInt(try_position.x/tile_scale), Mathf.RoundToInt(try_position.y/tile_scale));
        if (sorted_tiles.ContainsKey(position) && !sorted_tiles[position].walkable) {
            Instantiate(drop, spawn_drop_location.transform.position, Quaternion.identity);
        } else {
            Instantiate(drop, try_position + transform.position, Quaternion.identity);
        }
    }

    public void AddDrop(GameObject drop) {
        drops_to_spawn.Add(drop);
    }
}