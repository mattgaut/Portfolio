using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByrdmanHandler : EnemyHandler {

    [SerializeField] List<RocketSpawner> rocket_spawners;
    [SerializeField] HomingProjectile homing_projectile;
    [SerializeField] Vector3 missile_knockback;
    List<GameObject> active_rockets;

    [SerializeField] List<Electricity> electricity_nodes;
    [SerializeField] Vector3 electric_knockback;
    [SerializeField] float zap_round_length;

    [SerializeField] List<Spike> spikes;
    [SerializeField] Vector3 spike_knockback;
    [SerializeField] float spike_length;

    [SerializeField] List<Laser> lasers;
    [SerializeField] Vector3 laser_knockback;
    [SerializeField] float laser_length;
    [SerializeField] float laser_horizontal_move_speed;


    [SerializeField] float fall_speed, raise_speed, fall_distance;

    [SerializeField] float rest_time;

    [SerializeField] Buff has_energy_buff, no_energy_buff;

    

    int attacks_occuring;
    int cycle_count, max_cycles = 4;

    bool lever_pulled;
    bool yellow_button, blue_button, red_button, green_button;

    int sub_cycle_count;

    protected override void Ini() {
        base.Ini();
        active_rockets = new List<GameObject>();
        //SetActive(true);

        foreach (RocketSpawner rs in rocket_spawners) {
            rs.Ini(homing_projectile, RocketOnHit, target.center_mass);
        }
        foreach (Electricity en in electricity_nodes) {
            en.SetOnHit(ZapOnHit);
        }
        foreach (Spike spike in spikes) {
            spike.SetOnHit(SpikeOnHit);
        }
        foreach (Laser laser in lasers) {
            laser.SetOnHit(LaserOnHit);
        }
        attacks_occuring = 0;
        lever_pulled = false;
        enemy.SetDieEvent(DieEvent);
    }

    protected override IEnumerator AIRoutine() {
        yield return null;
        has_energy_buff.ApplyTo(enemy);
        List<Func<IEnumerator>> round_1_attacks = new List<Func<IEnumerator>>() { Rockets, Electric, SpikeAttack };
        List<Func<IEnumerator>> wall_attacks = new List<Func<IEnumerator>>() { SpikeAttack, Rockets };
        List<Func<IEnumerator>> vertical_attacks = new List<Func<IEnumerator>>() { LaserAttack, Electric };
        List<Func<IEnumerator>> round_3_attacks = new List<Func<IEnumerator>>() { LaserAttack, Electric, SpikeAttack };

        while (active) {
            // 1 Trap            
            sub_cycle_count = 0;
            round_1_attacks.Shuffle();
            StartCoroutine(round_1_attacks[0]());
            enemy.animator.SetTrigger("Blink");
            while (attacks_occuring > 0) {
                yield return null;
            }
            enemy.animator.SetTrigger("Spend");
            // 2 trap
            sub_cycle_count = 1;
            wall_attacks.Shuffle();
            vertical_attacks.Shuffle();
            StartCoroutine(wall_attacks[0]());
            StartCoroutine(vertical_attacks[0]());
            enemy.animator.SetTrigger("Blink");
            while (attacks_occuring > 0) {
                yield return null;
            }
            enemy.animator.SetTrigger("Spend");
            // 3 trap
            round_3_attacks.Shuffle();
            List<Func<IEnumerator>> attacks = new List<Func<IEnumerator>>() { Rockets, round_3_attacks[0], round_3_attacks[1] };
            sub_cycle_count = 2;
            attacks.Shuffle();
            StartCoroutine(attacks[0]());
            StartCoroutine(attacks[1]());
            StartCoroutine(attacks[2]());
            enemy.animator.SetTrigger("Blink");
            while (attacks_occuring > 0) {
                yield return null;
            }
            enemy.animator.SetTrigger("Spend");
            cycle_count++;
            // fall
            float original_height = transform.position.y;
            float fallen_distance = 0;
            while (fall_distance > fallen_distance) {
                float to_fall = fall_speed * Time.deltaTime;
                if (fall_distance < fallen_distance + to_fall) {
                    to_fall = fall_distance - fallen_distance;
                }
                fallen_distance += to_fall;
                enemy.transform.position += Vector3.down * to_fall;
                yield return null;
            }
            has_energy_buff.RemoveFrom(enemy);
            no_energy_buff.ApplyTo(enemy);
            enemy.animator.SetBool("Rest", true);

            yield return null;
            yield return null;
            yield return null;
            while (enemy.health.current/enemy.health > (1f - (float)cycle_count/max_cycles)) {
                yield return null;
            }
            // raise
            lever_pulled = false;
            enemy.animator.SetTrigger("Refill");
            enemy.animator.SetBool("Rest", false);
            no_energy_buff.RemoveFrom(enemy);
            has_energy_buff.ApplyTo(enemy);

            while (!lever_pulled) {
                yield return null;
            }

            while (fallen_distance > 0) {
                float to_raise = raise_speed * Time.deltaTime;
                if (0 > fallen_distance - to_raise) {
                    to_raise = fallen_distance;
                }
                fallen_distance -= to_raise;
                enemy.transform.position += Vector3.up * to_raise;
                yield return null;
            }
            transform.position = new Vector3(transform.position.x, original_height, transform.position.z);

            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator Rockets() {
        enemy.animator.SetTrigger("Yellow");
        attacks_occuring++;
        yield return null;
        while (!yellow_button) {
            yield return null;
        }
        yellow_button = false;
        if (sub_cycle_count <= 1) {
            yield return RocketPattern1();
        } else {
            if (UnityEngine.Random.Range(0,1f) < 0.5) {
                yield return RocketPattern1();
            } else {
                yield return RocketPattern1();
            }
        }

        while (active_rockets.Count > 0) {
            yield return null;
            for (int i = 0; i < active_rockets.Count; i++) {
                if (active_rockets[i] == null) {
                    active_rockets.RemoveAt(i);
                    i--;
                }
            }
        }
        attacks_occuring--;
    }

    IEnumerator RocketPattern1() {
        rocket_spawners[0].Ready();
        rocket_spawners[1].Ready();
        rocket_spawners[2].Ready();
        while (!rocket_spawners[0].can_spawn) {
            yield return null;
        }
        List<int> spawners = new List<int>() { 0, 1, 2 };
        spawners.Shuffle();

        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[0]].Spawn().gameObject);
        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[1]].Spawn().gameObject);
        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[2]].Spawn().gameObject);

        yield return new WaitForSeconds(0.5f);
        spawners.Shuffle();
        rocket_spawners[spawners[0]].Ready();
        rocket_spawners[spawners[1]].Ready();
        rocket_spawners[spawners[2]].Ready();
        while (!rocket_spawners[spawners[0]].can_spawn) {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[0]].Spawn().gameObject);
        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[1]].Spawn().gameObject);
        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[2]].Spawn().gameObject);
    }
    IEnumerator EasyRockets() {
        rocket_spawners[0].Ready();
        rocket_spawners[1].Ready();
        rocket_spawners[2].Ready();
        while (!rocket_spawners[0].can_spawn) {
            yield return null;
        }
        List<int> spawners = new List<int>() { 0, 1, 2 };
        spawners.Shuffle();

        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[0]].Spawn().gameObject);
        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[1]].Spawn().gameObject);
        yield return new WaitForSeconds(0.5f);
        active_rockets.Add(rocket_spawners[spawners[2]].Spawn().gameObject);
    }

    IEnumerator Electric() {
        enemy.animator.SetTrigger("Blue");
        attacks_occuring++;
        yield return null;
        while (!blue_button) {
            yield return null;
        }
        blue_button = false;
        if (sub_cycle_count == 0) {
            yield return HardElectric();
        } else if (sub_cycle_count == 1) {
            yield return ElectricityPattern1();
        } else if (sub_cycle_count == 2) {
            yield return EasyElectric();
        }

        yield return new WaitForSeconds(2f);
        attacks_occuring--;
    }

    IEnumerator ElectricityPattern1() {
        electricity_nodes[0].Raise();
        electricity_nodes[1].Raise();
        electricity_nodes[2].Raise();
        electricity_nodes[3].Raise();
        while (!electricity_nodes[0].can_begin_zap) {
            yield return null;
        }
        List<int> numbers = new List<int>() { 0, 1, 2, 3 };
        numbers.Shuffle();
        electricity_nodes[numbers[0]].Ready();
        electricity_nodes[numbers[1]].Ready();
        yield return new WaitForSeconds(2f);
        electricity_nodes[numbers[0]].Zap(zap_round_length);
        electricity_nodes[numbers[1]].Zap(zap_round_length);

        yield return new WaitForSeconds(zap_round_length);

        while (!electricity_nodes[numbers[0]].can_begin_zap) {
            yield return null;
        }

        numbers.Shuffle();
        electricity_nodes[numbers[0]].Ready();
        electricity_nodes[numbers[1]].Ready();
        yield return new WaitForSeconds(2f);
        electricity_nodes[numbers[0]].Zap(zap_round_length);
        electricity_nodes[numbers[1]].Zap(zap_round_length);

        yield return new WaitForSeconds(zap_round_length);

        while(!electricity_nodes[numbers[0]].can_begin_zap){
            yield return null;
        }

        electricity_nodes[0].Lower();
        electricity_nodes[1].Lower();
        electricity_nodes[2].Lower();
        electricity_nodes[3].Lower();
    }
    IEnumerator EasyElectric() {
        electricity_nodes[0].Raise();
        electricity_nodes[1].Raise();
        electricity_nodes[2].Raise();
        electricity_nodes[3].Raise();
        while (!electricity_nodes[0].can_begin_zap) {
            yield return null;
        }
        List<int> numbers = new List<int>() { 0, 1, 2, 3 };
        numbers.Shuffle();
        electricity_nodes[numbers[0]].Ready();
        yield return new WaitForSeconds(2f);
        electricity_nodes[numbers[0]].Zap(zap_round_length);

        yield return new WaitForSeconds(zap_round_length);

        while (!electricity_nodes[numbers[0]].can_begin_zap) {
            yield return null;
        }

        numbers.Shuffle();
        electricity_nodes[numbers[0]].Ready();
        yield return new WaitForSeconds(2f);
        electricity_nodes[numbers[0]].Zap(zap_round_length);

        yield return new WaitForSeconds(zap_round_length);

        while (!electricity_nodes[numbers[0]].can_begin_zap) {
            yield return null;
        }

        electricity_nodes[0].Lower();
        electricity_nodes[1].Lower();
        electricity_nodes[2].Lower();
        electricity_nodes[3].Lower();
    }
    IEnumerator HardElectric() {
        electricity_nodes[0].Raise();
        electricity_nodes[1].Raise();
        electricity_nodes[2].Raise();
        electricity_nodes[3].Raise();
        while (!electricity_nodes[0].can_begin_zap) {
            yield return null;
        }
        List<int> numbers = new List<int>() { 0, 1, 2, 3 };
        numbers.Shuffle();
        electricity_nodes[numbers[0]].Ready();
        electricity_nodes[numbers[1]].Ready();
        electricity_nodes[numbers[2]].Ready();
        yield return new WaitForSeconds(2f);
        electricity_nodes[numbers[0]].Zap(zap_round_length);
        electricity_nodes[numbers[1]].Zap(zap_round_length);
        electricity_nodes[numbers[2]].Zap(zap_round_length);

        yield return new WaitForSeconds(zap_round_length);

        while (!electricity_nodes[numbers[0]].can_begin_zap) {
            yield return null;
        }

        numbers.Shuffle();
        electricity_nodes[numbers[0]].Ready();
        electricity_nodes[numbers[1]].Ready();
        electricity_nodes[numbers[2]].Ready();
        yield return new WaitForSeconds(2f);
        electricity_nodes[numbers[0]].Zap(zap_round_length);
        electricity_nodes[numbers[1]].Zap(zap_round_length);
        electricity_nodes[numbers[2]].Zap(zap_round_length);


        yield return new WaitForSeconds(zap_round_length);


        while (!electricity_nodes[numbers[0]].can_begin_zap) {
            yield return null;
        }

        electricity_nodes[0].Lower();
        electricity_nodes[1].Lower();
        electricity_nodes[2].Lower();
        electricity_nodes[3].Lower();
    }

    IEnumerator SpikeAttack() {
        enemy.animator.SetTrigger("Green");
        attacks_occuring++;
        yield return null;
        while (!green_button) {
            yield return null;
        }
        green_button = false;
        yield return SpikePattern1();

        yield return new WaitForSeconds(1f);
        attacks_occuring--;
    }

    IEnumerator SpikePattern1() {
        spikes[0].Load();
        spikes[1].Load();
        while (!spikes[0].can_spike) {
            yield return null;
        }
        List<int> numbers = new List<int>() { 0, 1 };
        numbers.Shuffle();
        spikes[numbers[0]].ShootSpike(spike_length);
        yield return null;
        while (!spikes[numbers[0]].can_spike) {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        spikes[numbers[1]].ShootSpike(spike_length);
        yield return null;
        while (!spikes[numbers[1]].can_spike) {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        spikes.Shuffle();
        spikes[numbers[1]].ShootSpike(spike_length);
        yield return null;
        while (!spikes[numbers[1]].can_spike) {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        spikes[0].Unload();
        spikes[1].Unload();
    }

    IEnumerator LaserAttack() {
        enemy.animator.SetTrigger("Red");
        attacks_occuring++;
        yield return null;
        while (!red_button) {
            yield return null;
        }
        red_button = false;
        yield return LaserPattern1();

        yield return new WaitForSeconds(2f);
        attacks_occuring--;
    }

    IEnumerator LaserPattern1() {
        lasers[0].Open();
        lasers[1].Open();

        List<int> numbers = new List<int>() { 0, 1 };
        numbers.Shuffle();
        lasers[numbers[0]].Ready();
        yield return null;
        while (!lasers[numbers[0]].can_fire) {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        lasers[numbers[0]].Fire(laser_length);

        yield return new WaitForSeconds(laser_length + 0.25f);

        lasers[numbers[1]].Ready();
        yield return null;
        while (!lasers[numbers[1]].can_fire) {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        lasers[numbers[1]].Fire(laser_length);

        yield return new WaitForSeconds(laser_length + 0.25f);

        lasers[0].Close();
        lasers[1].Close();
    }

    void RocketOnHit(IDamageable hit, Attack hit_by) {
        enemy.DealDamage(enemy.power, hit);
        Vector3 knockback = missile_knockback;
        knockback.x *= Mathf.Sign(hit.gameObject.transform.position.x - hit_by.gameObject.transform.position.x);
        hit.TakeKnockback(enemy, knockback);
    }

    void ZapOnHit(IDamageable hit, Attack hit_by) {
        enemy.DealDamage(enemy.power, hit);
        Vector3 knockback = missile_knockback;
        knockback.x *= Mathf.Sign(UnityEngine.Random.Range(0f, 1f) - 0.5f);
        hit.TakeKnockback(enemy, knockback);
    }
    void SpikeOnHit(IDamageable hit, Attack hit_by) {
        enemy.DealDamage(enemy.power, hit);
        hit.TakeKnockback(enemy, spike_knockback);
    }

    void LaserOnHit(IDamageable hit, Attack hit_by) {
        enemy.DealDamage(enemy.power, hit);
        Vector3 knockback = laser_knockback;
        knockback.x *= Mathf.Sign(hit.gameObject.transform.position.x - hit_by.gameObject.transform.position.x);
        hit.TakeKnockback(enemy, knockback);
    }

    public void LeverPulled() {
        lever_pulled = true;
    }

    public void PressBlue() {
        blue_button = true;
    }
    public void PressRed() {
        red_button = true;
    }
    public void PressYellow() {
        yellow_button = true;
    }
    public void PressGreen() {
        green_button = true;
    }

    IEnumerator DieEvent() {
        StopCoroutine(ai_routine);
        yield return UIHandler.StartEndCrawl();
        GetComponentInChildren<Animator>().transform.SetParent(null);
    }
}
