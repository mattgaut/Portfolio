using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomLayout : RoomLayout {

    [SerializeField]
    Spawn next_level_tile;
    [SerializeField]
    Spawn boss_enemy;
    Enemy boss;

    [SerializeField]
    GameObject item_pedastal;
    GameObject item;

    public override bool can_unlock {
        get {
            return enemies.Count == 0 && boss.dead;
        }
    }

    protected override void Awake() {
        base.Awake();

        boss = boss_enemy.SpawnObject().GetComponent<Enemy>();
        enemies.Add(boss, boss.transform.position);
        next_level_tile.gameObject.SetActive(false);
        item_pedastal.SetActive(false);
    }
    protected override void Start() {
        boss.SetRoom(this);
        if (boss.GetComponent<PathfinderAgent>() != null) {
            boss.GetComponent<PathfinderAgent>().Initiate();
        }
        base.Start();
    }

    protected override void DisableRoom() {
        base.DisableRoom();
        if (boss)
            boss.gameObject.SetActive(false);
    }

    protected override void EnableRoom() {
        base.EnableRoom();
        if (boss) {

            boss.gameObject.SetActive(true);
            boss.transform.position = enemies[boss];
            UIController.instance.DisplayBossHealthBar(boss);
        }
    }

    public override void RemoveEnemy(Enemy e) {

        enemies.Remove(e);
        enemy_objects.Remove(e.gameObject);

        if (enemies.Count == 0 && boss.dead) {
            SpawnObjects();
        }
    }

    protected override void SpawnObjects() {
        base.SpawnObjects();

        next_level_tile.gameObject.SetActive(true);
        item_pedastal.SetActive(true);
        next_level_tile.SpawnObject();
    }

    public void SetItem(GameObject g) {
        item_pedastal.SetActive(true);
        item = Instantiate(g, item_pedastal.transform, false);
        item_pedastal.SetActive(false);
    }
}
