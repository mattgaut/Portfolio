using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomBuilder : MonoBehaviour {
    [SerializeField] bool boss_room;
    [SerializeField] TileSet tileset;
    [SerializeField] Tile square, upward_triangle, platform, downward_triangle;
    [SerializeField] Block block;
    TileType selected_tile_type;
    Tile current;

    Room edited_room;
    [SerializeField] Room base_room;

    [SerializeField] Vector2Int bounds;

    Dictionary<Vector2Int, Tile> placed_tiles;
    Dictionary<Vector2Int, Block> placed_blocks;

    bool fill_next;

    void Start() {
        placed_tiles = new Dictionary<Vector2Int, Tile>();

        placed_blocks = new Dictionary<Vector2Int, Block>();
        Room load = FindObjectOfType<Room>();

        if (load != null) {
            LoadRoom(load);
            if (boss_room) {
                edited_room.gameObject.AddComponent<BossRoom>();
                BossRoom br = edited_room.GetComponent<BossRoom>();
                br.Copy(edited_room);
                Destroy(edited_room);
                edited_room = br;
            }
        } else {
            edited_room = Instantiate(base_room);
            if (boss_room) {
                edited_room.gameObject.AddComponent<BossRoom>();
                BossRoom br = edited_room.GetComponent<BossRoom>();
                br.Copy(edited_room);
                Destroy(edited_room);
                edited_room = br;
            }
            PlaceBlock(Vector2Int.zero);
        }
        current = square;
    }
    private void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (!fill_next && Input.GetKey(KeyCode.Mouse0)) {
                PlaceTile();
            } else if (fill_next && Input.GetKey(KeyCode.Mouse0)) {
                FillTiles(Vec2ToVec2Int(GetMousePosition()));
            }
            if (Input.GetKey(KeyCode.Mouse1)) {
                if(InBounds(Vec2ToVec2Int(GetMousePosition()))) RemoveTile(Vec2ToVec2Int(GetMousePosition()));
            }
        }
    }

    Vector2 GetMousePosition() {
        Vector2 v = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        v = new Vector3(Mathf.Round(v.x), Mathf.Round(v.y));
        return v;
    }

    void FillTiles(Vector2Int start) {
        List<Vector2Int> list = new List<Vector2Int>();
        list.Add(start);
        while (list.Count > 0) {
            if (!placed_tiles.ContainsKey(list[0]) && InBounds(list[0])) {
                PlaceTile(current, list[0]);
                list.Add(list[0] + Vector2Int.down);
                list.Add(list[0] + Vector2Int.right);
                list.Add(list[0] + Vector2Int.left);
                list.Add(list[0] + Vector2Int.up);
            }
            list.RemoveAt(0);
        }
        fill_next = false;
    }

    void PlaceTile() {
        PlaceTile(current, Vec2ToVec2Int(GetMousePosition()));
    }

    void PlaceTile(Tile t, Vector2Int position) {
        if (!placed_tiles.ContainsKey(Vec2ToVec2Int(position)) && InBounds(position)) {
            ForcePlaceTile(t, position);
        }
    }

    void ForcePlaceTile(Tile t, Vector2Int position) {
        t = Instantiate(t);
        t.transform.SetParent(edited_room.block_object.transform);
        AddTile(t, position);
    }

    void AddTile(Tile t, Vector2Int position) {
        t.position = Vec2ToVec2Int(position);
        placed_tiles.Add(Vec2ToVec2Int(position), t);
        t.LoadSprite(tileset);
        t.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), 0);
        if (t.tile_type != TileType.Platform) {
            if (Occupied(t.position + Vector2Int.up) && placed_tiles[t.position + Vector2Int.up].tile_type != TileType.Platform) {
                t.SetTopBorder(false);
                placed_tiles[t.position + Vector2Int.up].SetBottomBorder(false);
            } else {
                t.SetTopBorder(true);
            }
            if (Occupied(t.position + Vector2Int.down) && placed_tiles[t.position + Vector2Int.down].tile_type != TileType.Platform) {
                t.SetBottomBorder(false);
                placed_tiles[t.position + Vector2Int.down].SetTopBorder(false);
            } else {
                t.SetBottomBorder(true);
            }
            if (Occupied(t.position + Vector2Int.left) && placed_tiles[t.position + Vector2Int.left].tile_type != TileType.Platform) {
                t.SetLeftBorder(false);
                placed_tiles[t.position + Vector2Int.left].SetRightBorder(false);
            } else {
                t.SetLeftBorder(true);
            }
            if (Occupied(t.position + Vector2Int.right) && placed_tiles[t.position + Vector2Int.right].tile_type != TileType.Platform) {
                t.SetRightBorder(false);
                placed_tiles[t.position + Vector2Int.right].SetLeftBorder(false);
            } else {
                t.SetRightBorder(true);
            }
        }
    }

    void RemoveTile(Vector2Int position, bool destroy = true) {
        if (placed_tiles.ContainsKey(position)) {
            if (destroy)Destroy(placed_tiles[position].gameObject);
            placed_tiles.Remove(position);

            if (placed_tiles.ContainsKey(position + Vector2Int.up)) {
                placed_tiles[Vector2Int.up + position].SetBottomBorder(true);
            }
            if (placed_tiles.ContainsKey(position + Vector2Int.right)) {
                placed_tiles[Vector2Int.right + position].SetLeftBorder(true);
            }
            if (placed_tiles.ContainsKey(position + Vector2Int.left)) {
                placed_tiles[Vector2Int.left + position].SetRightBorder(true);
            }
            if (placed_tiles.ContainsKey(position + Vector2Int.down)) {
                placed_tiles[Vector2Int.down + position].SetTopBorder(true);
            }
        }
    }
    public void PlaceNewBlocksLeft() {
        List<Vector2Int> new_positions = new List<Vector2Int>();
        foreach (Block b in edited_room.template.GetBlocks()) {
            if (b.local_position.x == edited_room.template.Xmin()) {
                new_positions.Add(b.local_position + Vector2Int.left);
            }
        }
        foreach (Vector2Int v in new_positions) {
            PlaceBlock(v);
        }
        TrimBlocks();
    }
    public void PlaceNewBlocksRight() {
        List<Vector2Int> new_positions = new List<Vector2Int>();
        foreach (Block b in edited_room.template.GetBlocks()) {
            if (b.local_position.x == edited_room.template.Xmax()) {
                new_positions.Add(b.local_position + Vector2Int.right);
            }
        }
        foreach (Vector2Int v in new_positions) {
            PlaceBlock(v);
        }
        TrimBlocks();
    }
    public void PlaceNewBlocksUp() {
        List<Vector2Int> new_positions = new List<Vector2Int>();
        foreach (Block b in edited_room.template.GetBlocks()) {
            if (b.local_position.y == edited_room.template.Ymax()) {
                new_positions.Add(b.local_position + Vector2Int.up);
            }
        }
        foreach (Vector2Int v in new_positions) {
            PlaceBlock(v);
        }
        TrimBlocks();
    }
    public void PlaceNewBlocksDown() {
        List<Vector2Int> new_positions = new List<Vector2Int>();
        foreach (Block b in edited_room.template.GetBlocks()) {
            if (b.local_position.y == edited_room.template.Ymin()) {
                new_positions.Add(b.local_position + Vector2Int.down);
            }
        }
        foreach (Vector2Int v in new_positions) {
            PlaceBlock(v);
        }
        TrimBlocks();
    }
    void PlaceBlock(Vector2Int block_position) {
        Block new_block = Instantiate(block, (Vector2)(bounds * block_position), Quaternion.identity);
        new_block.gameObject.name = "Block(" + block_position.x + "," + block_position.y + ")";
        new_block.gameObject.transform.SetParent(edited_room.block_object.transform, true);
        placed_blocks.Add(block_position, new_block);
        new_block.SetBlock(edited_room, block_position);
        edited_room.template.AddBlock(block_position, new_block);
        foreach (Tile t in new_block.GetComponentsInChildren<Tile>(true)) {
            t.position = t.position + (bounds * block_position);
        }
        foreach (Tile t in new_block.GetComponentsInChildren<Tile>()) {
            AddTile(t, t.position);
        }
        foreach (DoorToggle t in new_block.GetComponentsInChildren<DoorToggle>()) {
            t.SetDoorPosition(block_position);
        }
    }
    void TrimBlocks() {
        foreach (Vector2Int v in placed_blocks.Keys) {
            if (placed_blocks.ContainsKey(v + Vector2Int.right)) {
                foreach (Tile t in placed_blocks[v].TrimWall(Pathway.RIGHT)) {
                    RemoveTile(t.position, false);
                }
                foreach (Tile t in placed_blocks[v + Vector2Int.right].TrimWall(Pathway.LEFT)) {
                    RemoveTile(t.position, false);
                }
            }
            if (placed_blocks.ContainsKey(v + Vector2Int.left)) {
                foreach (Tile t in placed_blocks[v].TrimWall(Pathway.LEFT)) {
                    RemoveTile(t.position, false);
                }
                foreach (Tile t in placed_blocks[v + Vector2Int.left].TrimWall(Pathway.RIGHT)) {
                    RemoveTile(t.position, false);
                }
            }
            if (placed_blocks.ContainsKey(v + Vector2Int.down)) {
                foreach (Tile t in placed_blocks[v].TrimWall(Pathway.BOTTOM)) {
                    RemoveTile(t.position, false);
                }
                foreach (Tile t in placed_blocks[v + Vector2Int.down].TrimWall(Pathway.TOP)) {
                    RemoveTile(t.position, false);
                }
            }
            if (placed_blocks.ContainsKey(v + Vector2Int.up)) {
                foreach (Tile t in placed_blocks[v].TrimWall(Pathway.TOP)) {
                    RemoveTile(t.position, false);
                }
                foreach (Tile t in placed_blocks[v + Vector2Int.up].TrimWall(Pathway.BOTTOM)) {
                    RemoveTile(t.position, false);
                }
            }
        }
    }
    public void SetTileType(TileType t) {
        selected_tile_type = t;
        if (selected_tile_type == TileType.Square) {
            current = square;
        } else if (selected_tile_type == TileType.Platform) {
            current = platform;
        } else if (selected_tile_type == TileType.UpwardTriangle) {
            current = upward_triangle;
        } else if (selected_tile_type == TileType.DownwardTriangle) {
            current = downward_triangle;
        }
    }
    public void SetTileType(UnityEngine.UI.Dropdown drop) {
        SetTileType((TileType)drop.value);
    }
    public void TogglePathway(DoorToggle dt) {
        Block b = edited_room.template.GetBlock(dt.Position().position);
        b.SetPathway(dt.Position().pathway, dt.is_doorway);
        foreach (Tile t in dt.GetComponentsInChildren<Tile>(true)) {
            if (t.gameObject.activeSelf) {
                RemoveTile(t.position, false);
                t.gameObject.SetActive(false);
            } else {
                t.gameObject.SetActive(true);
            }
        }
        foreach (Tile t in dt.GetComponentsInChildren<Tile>()) {
            if (t.gameObject.activeSelf) {
                AddTile(t, t.position);
            }
        }
    }
    Vector2Int Vec2ToVec2Int(Vector2 vec) {
        return new Vector2Int((int)vec.x, (int)vec.y);
    }
    void LoadRoom(Room r) {
        edited_room = Instantiate(base_room);
        edited_room.CopyRoomSet(r);
        foreach (Block block in r.template.GetBlocks()) {
            PlaceBlock(block.local_position);
        }
        TrimBlocks();
        foreach (Tile t in r.GetComponentsInChildren<Tile>()) {
            SetTileType(t.tile_type);
            PlaceTile(current, t.position);
        }
        foreach (Block block in edited_room.template.GetBlocks()) {
            foreach (DoorToggle t in block.GetComponentsInChildren<DoorToggle>()) {
                t.SetDoorPosition(block.local_position);
            }
        }
        Destroy(r.gameObject);
    }

    bool InBounds(Vector2Int v) {
        return v.x < (edited_room.template.Xmax() * bounds.x) + (bounds.x )/2 && v.x > (edited_room.template.Xmin() * bounds.x) - (bounds.x - 1)/2 && v.y < (edited_room.template.Ymax() * bounds.y) + (bounds.y)/2 && v.y > (edited_room.template.Ymin() * bounds.y) - (bounds.y - 1)/2;
    }

    bool Occupied(Vector2Int v) {
        return placed_tiles.ContainsKey(v);
    }

    public void FillNext(bool f) {
        fill_next = f;
    }
    public void BeginSave() {
        foreach (DoorToggle dt in edited_room.GetComponentsInChildren<DoorToggle>()) {
            edited_room.template.GetBlock(dt.Position().position).SetPathway(dt.Position().pathway, dt.is_doorway);
            Destroy(dt);
            Destroy(dt.GetComponent<BoxCollider2D>());
            Destroy(dt.GetComponent<SpriteRenderer>());
        }
        Debug.Break();

    }
}
