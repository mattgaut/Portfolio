using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : Room {

    [SerializeField] Block entrance_block, fighting_block, leaving_block;

    [SerializeField] Enemy boss;

    [SerializeField] Door door_in, door_to_next_floor, door_lock_in;

    [SerializeField] ItemSpawner reward_spawner;

    [SerializeField] AudioClip boss_theme;

    public ItemSpawner reward {
        get { return reward_spawner; }
    }

    bool fighting, boss_dead;

    [SerializeField] Collider2D boss_blocker;

    public void OnEnterArena() {
        door_in.Close();
        if (door_to_next_floor) door_to_next_floor.Close();

        fighting = true;

        boss.GetComponent<EnemyDisplay>().Enable(true);
        boss.SetRoom(this);
        boss.GetComponent<EnemyHandler>().SetActive(true);

        door_in.SetHardLocked(true);
        door_lock_in.SetHardLocked(true);
        if (door_to_next_floor) door_to_next_floor.SetHardLocked(true);
        if (boss_theme != null) SoundManager.PlaySong(boss_theme);
    }

    public void OnBossDefeated() {
        fighting = false;
        boss_dead = true;

        if (door_to_next_floor) {
            door_to_next_floor.SetHardLocked(false);
            door_to_next_floor.Open();
        }


        if (boss_blocker) boss_blocker.enabled = false;
    }

    public override Vector3 ClampToCameraBounds(Vector3 position) {
        if (!fighting && !boss_dead) {
            return ClampToEntranceBlock(position);
        } else if (!fighting && boss_dead) {
            return ClampToFightingAndExitBlock(position);
        } else {
            return ClampToFightingBlock(position);
        }
    }

    Vector3 ClampToEntranceBlock(Vector3 position) {
        Vector3 local_position = position - transform.position;
        local_position.x = entrance_block.local_position.x * bound_x;
        local_position.y = entrance_block.local_position.y * bound_y;
        local_position += new Vector3(0.5f, 0.5f);
        return local_position + transform.position;
    }
    Vector3 ClampToFightingBlock(Vector3 position) {
        Vector3 local_position = position - transform.position;
        local_position.x = fighting_block.local_position.x * bound_x;
        local_position.y = fighting_block.local_position.y * bound_y;
        local_position += new Vector3(0.5f, 0.5f);
        return local_position + transform.position;
    }
    Vector3 ClampToFightingAndExitBlock(Vector3 position) {
        Vector3 local_position = position - transform.position;
        float x_min, y_min, x_max, y_max = 0;
        x_min = Mathf.Min(fighting_block.local_position.x, leaving_block.local_position.x);
        y_min = Mathf.Min(fighting_block.local_position.y, leaving_block.local_position.y);
        x_max = Mathf.Max(fighting_block.local_position.x, leaving_block.local_position.x);
        y_max = Mathf.Max(fighting_block.local_position.y, leaving_block.local_position.y);

        if (local_position.x > x_max * bound_x) {
            local_position.x = x_max * bound_x;
        } else if (local_position.x < x_min * bound_x) {
            local_position.x = x_min * bound_x;
        }
        if (local_position.y > y_max * bound_y) {
            local_position.y = y_max * bound_y;
        } else if (local_position.y < y_min * bound_y) {
            local_position.y = y_min * bound_y;
        }
        
        local_position += new Vector3(0.5f, 0.5f);
        return local_position + transform.position;
    }

    public override void RemoveEnemy(Enemy enemy) {
        base.RemoveEnemy(enemy);
        if (enemy == boss) {
            OnBossDefeated();
        }
    }
}
