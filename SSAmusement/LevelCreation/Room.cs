using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    [SerializeField] protected Template _template;
    protected Vector2Int _grid_position;
    [SerializeField] GameObject block_container;
    GameObject room_set;
    [SerializeField] List<GameObject> room_sets;

    Dictionary<Enemy, Vector3> enemies;

    public GameObject block_object { get { return block_container; } }

    public Template template { get { return _template; } }
    public Vector2Int grid_position { get { return _grid_position; } set { _grid_position = value; } }
    [SerializeField] protected float bound_x = 18, bound_y = 10;
    public float width { get { return bound_x; } }
    public float height { get { return bound_y; } }

    BoundaryBox boundary_box;
   
    public class DoorPosition : IEqualityComparer<DoorPosition> {
        public Vector2Int position {
            get; private set;
        }
        public Pathway pathway {
            get; private set;
        }

        public DoorPosition(Vector2Int pos, Pathway path) {
            position = pos;
            pathway = path;
        }

        public bool Equals(DoorPosition x, DoorPosition y) {
            return x.position == y.position && x.pathway == y.pathway;
        }

        public int GetHashCode(DoorPosition obj) {
            return obj.position.GetHashCode() * obj.pathway.GetHashCode();
        }
    }
    [System.Serializable]
    public class Template {
        [SerializeField] List<Block> blocks_b;
        Dictionary<Vector2Int, Block> blocks;

        public Template(Template other) {
            blocks_b = other.blocks_b;
            Ini();
        }

        int ymax, ymin, xmax, xmin;

        public Block GetBlock(Vector2Int coords) {
            if (blocks == null) {
                Ini();
            }
            if (blocks.ContainsKey(coords)) {
                return blocks[coords];
            }
            return null;
        }
        public List<Block> GetBlocks() {
            return new List<Block>(blocks_b);
        }
        public List<Vector2Int> GetCoordinateList() {
            if (blocks == null) {
                Ini();
            }
            return new List<Vector2Int>(blocks.Keys);
        }
        public void Ini() {
            if (blocks_b == null) {
                blocks_b = new List<Block>();
            }
            blocks = new Dictionary<Vector2Int, Block>();
            for (int i = 0; i < blocks_b.Count && i < blocks_b.Count; i++) {
                blocks.Add(blocks_b[i].local_position, blocks_b[i]);
            }
            blocks_b.Sort((a, b) => a.local_position.x - b.local_position.x);
            if (blocks_b.Count > 0) {
                xmin = blocks_b[0].local_position.x;
                blocks_b.Sort((a, b) => -a.local_position.x + b.local_position.x);
                xmax = blocks_b[0].local_position.x;
                blocks_b.Sort((a, b) => -a.local_position.y + b.local_position.y);
                ymax = blocks_b[0].local_position.y;
                blocks_b.Sort((a, b) => a.local_position.y - b.local_position.y);
                ymin = blocks_b[0].local_position.y;
            }
        }
        public void AddBlock(Vector2Int v, Block b) {
            blocks_b.Add(b);
            Ini();
        }
        public int Ymin() {
            return ymin;
        }
        public int Ymax() {
            return ymax;
        }
        public int Xmax() {
            return xmax;
        }
        public int Xmin() {
            return xmin;
        }
        public int Width() {
            return xmax - xmin + 1;
        }
        public int Height() {
            return ymax - ymin + 1;
        }
        public Vector2 Center() {
            return new Vector2((xmax + xmin)/2f, (ymin + ymax)/2f);
        }
    }



    private void Awake() {
        if(_template != null) {
            _template.Ini();
            foreach (Block b in template.GetBlocks()) {
                b.SetRoom(this);
            }
        }
        boundary_box = GetComponentInChildren<BoundaryBox>();
        if (boundary_box != null) {
            boundary_box.Set(this);
            boundary_box.Disable();
        }
        enemies = new Dictionary<Enemy, Vector3>();
    }

    void DisablePathway(Pathway pathway, Vector2Int pos) {
        Block block = _template.GetBlock(pos);
        if (block == null) return;
        block.DisablePathway(pathway);
    }

    public void SetSize(float x, float y) {
        GetComponent<BoxCollider2D>().size = new Vector2((template.Xmax() - template.Xmin() + 1) * x - 1, (template.Ymax() - template.Ymin() + 1) * y - 1);
        GetComponent<BoxCollider2D>().offset = new Vector2(((template.Xmax() + template.Xmin()) * x / 2) + (0.5f * (1 - (x % 2))), ((template.Ymax() + template.Ymin()) * y / 2) + (0.5f * (1 - (y % 2))));
        bound_x = x;
        bound_y = y;
    }

    public virtual Vector3 ClampToCameraBounds(Vector3 position) {
        Vector3 local_position = position - transform.position;
        if (local_position.x < _template.Xmin() * bound_x) {
            local_position.x = _template.Xmin() * bound_x;
        } else if (local_position.x > _template.Xmax() * bound_x) {
            local_position.x = _template.Xmax() * bound_x;
        }
        if (local_position.y < _template.Ymin() * bound_y) {
            local_position.y = _template.Ymin() * bound_y;
        } else if (local_position.y > _template.Ymax() * bound_y) {
            local_position.y = _template.Ymax() * bound_y;
        }
        local_position += new Vector3(0.5f, 0.5f);
        return local_position + transform.position;
    }
    public virtual Vector3 ClampToObjectBounds(Vector3 position) {
        Vector3 local_position = position - transform.position;
        int half_bound = (int)(bound_x / 2);
        if (local_position.x < (_template.Xmin() * bound_x) + half_bound) {
            local_position.x = _template.Xmin() * bound_x + half_bound;
        } else if (local_position.x > (_template.Xmax() * bound_x) + half_bound) {
            local_position.x = (_template.Xmax() * bound_x) + half_bound;
        }
        half_bound = (int)(bound_y / 2);
        if (local_position.y < (_template.Ymin() * bound_y) + half_bound) {
            local_position.y = (_template.Ymin() * bound_y) + half_bound;
        } else if (local_position.y > (_template.Ymax() * bound_y) + half_bound) {
            local_position.y = (_template.Ymax() * bound_y) + half_bound;
        }
        local_position += new Vector3(0.5f, 0.5f);
        return local_position + transform.position;
    }
    public virtual Vector3 ClampMovementToObjectBounds(Vector3 origin, Vector3 movement) {
        Vector3 local_position = (origin + movement) - transform.position;
        int half_bound = (int)(bound_x / 2);
        if (local_position.x < (_template.Xmin() * bound_x) - half_bound) {
            local_position.x = _template.Xmin() * bound_x - half_bound;
        } else if (local_position.x > (_template.Xmax() * bound_x) + half_bound) {
            local_position.x = (_template.Xmax() * bound_x) + half_bound;
        }
        half_bound = (int)(bound_y / 2);
        if (local_position.y < (_template.Ymin() * bound_y) - half_bound) {
            local_position.y = (_template.Ymin() * bound_y) - half_bound;
        } else if (local_position.y > (_template.Ymax() * bound_y) + half_bound) {
            local_position.y = (_template.Ymax() * bound_y) + half_bound;
        }
        return (local_position + transform.position) - origin;
    }
    public virtual bool ClampToObjectBounds(ref Vector3 position) {
        Vector3 local_position = position - transform.position;
        int half_bound = (int)(bound_x / 2);
        bool changed = false;
        if (local_position.x < (_template.Xmin() * bound_x) + half_bound) {
            changed = true;
            local_position.x = _template.Xmin() * bound_x + half_bound;
        } else if (local_position.x > (_template.Xmax() * bound_x) + half_bound) {
            changed = true;
            local_position.x = (_template.Xmax() * bound_x) + half_bound;
        }
        half_bound = (int)(bound_y / 2);
        if (local_position.y < (_template.Ymin() * bound_y) + half_bound) {
            changed = true;
            local_position.y = (_template.Ymin() * bound_y) + half_bound;
        } else if (local_position.y > (_template.Ymax() * bound_y) + half_bound) {
            changed = true;
            local_position.y = (_template.Ymax() * bound_y) + half_bound;
        }
        return !changed;
    }

    public void LoadTileSet(TileSet ts) {
        foreach (Tile t in GetComponentsInChildren<Tile>(true)) {
            t.LoadSprite(ts);
        }
    }

    public void SpawnRandomRoomset() {
        if (room_set != null && room_sets.Count > 0) {
            Destroy(room_set);
        }

        enemies = new Dictionary<Enemy, Vector3>();

        if (room_sets.Count > 0) {
            room_set = Instantiate(room_sets[Random.Range(0, room_sets.Count)]);
            room_set.transform.SetParent(transform);
            room_set.transform.position = transform.position;
            foreach (EnemySpawner es in room_set.GetComponentsInChildren<EnemySpawner>()) {
                Enemy e = es.Spawn();
                enemies.Add(e, e.transform.position);
                e.SetRoom(this);
            }
            foreach (Spawner s in room_set.GetComponentsInChildren<Spawner>()) {
                s.Spawn();
            }
        }
    }

    public virtual void OnActivate() {
        foreach (Enemy enemy in enemies.Keys) {
            enemy.GetComponent<EnemyHandler>().SetActive(true);
        }
        boundary_box.Enable();
    }
    public virtual void OnDeactivate() {
        foreach (Enemy enemy in enemies.Keys) {
            enemy.GetComponent<EnemyHandler>().SetActive(false);
            enemy.transform.position = enemies[enemy];
        }
        boundary_box.Disable();
    }

    public virtual void Copy(Room r) {
        bound_x = r.bound_x;
        bound_y = r.bound_y;
        _template = new Template(r.template);
        grid_position = r.grid_position;
        block_container = r.block_container;
        CopyRoomSet(r);
    }

    public void CopyRoomSet(Room r) {
        room_sets = new List<GameObject>();
        foreach (GameObject go in r.room_sets) {
            room_sets.Add(go);
        }
    }

    public virtual void RemoveEnemy(Enemy enemy) {
        if (enemies.ContainsKey(enemy)) {
            enemies.Remove(enemy);
        }
    }

    public Vector2 top_left {
        get { return new Vector2((_template.Xmin() * bound_x) - (bound_x / 2) + 0.5f, _template.Ymax() * bound_y + (bound_y / 2) + 0.5f); }
    }
    public Vector2 top_right {
        get { return new Vector2((_template.Xmax() * bound_x) + (bound_x / 2) + 0.5f, _template.Ymax() * bound_y + (bound_y / 2) + 0.5f); }
    }
    public Vector2 bottom_left {
        get { return new Vector2((_template.Xmin() * bound_x) - (bound_x / 2) + 0.5f, _template.Ymin() * bound_y - (bound_y / 2) + 0.5f); }
    }
    public Vector2 bottom_right {
        get { return new Vector2((_template.Xmax() * bound_x) + (bound_x / 2) + 0.5f, _template.Ymin() * bound_y - (bound_y / 2) + 0.5f); }
    }
}
