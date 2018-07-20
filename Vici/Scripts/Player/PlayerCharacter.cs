using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class PlayerCharacter : Character {

    protected Spell ability1, ability2, ability3, ability4;
    protected BasicAttack basic_attack;
    [SerializeField]
    GameObject inventory_holder;

    public BaseStat intelligence {
        get; protected set;
    }
    public BaseStat strength {
        get; protected set;
    }
    public BaseStat dexterity {
        get; protected set;
    }
    public BaseStat vitality {
        get; protected set;
    }

    public Inventory inventory {
        get; private set;
    }

    protected override void Start() {
        base.Start();
        LoadStats();
        LoadSkills();

        inventory = new Inventory(inventory_holder);

        DontDestroyOnLoad(this);
    }
    protected abstract void LoadSkills();
    protected virtual void LoadStats() {
        max_health = new FormulaStat(() => (int)vitality.value);
    }

    bool using_ability;

    public void TryUseBasicAttack() {
        if (basic_attack.CanUseAbility()) {
            basic_attack.UseAbility();
        }
    }
    protected abstract void UseBasicAttack();

    public void TryUseAbility1() {
        if (ability1.CanUseAbility()) {
            ability1.UseAbility();
        }
    }
    protected abstract void UseAbility1();

    public void TryUseAbility2() {
        if (ability2.CanUseAbility()) {
            ability2.UseAbility();
        }
    }
    protected abstract void UseAbility2();

    public void TryUseAbility3() {
        if (ability3.CanUseAbility()) {
            ability3.UseAbility();
        }
    }
    protected abstract void UseAbility3();

    public void TryUseAbility4() {
        if (ability4.CanUseAbility()) {
            ability4.UseAbility();
        }
    }
    protected abstract void UseAbility4();

    protected override void Die() {
        dead = true;
        UIController.instance.ShowDeathScreen();
    }

    public override float TakeDamage(float dmg) {
        if (dmg == 0 || invincible) {
            return 0;
        }
        base.TakeDamage(1);
        UIController.instance.FlashScreen(Color.red - new Color(0,0,0,0.7f));
        StartCoroutine(IFrames(1.5f));
        return 1;
    }

    bool invincible;
    IEnumerator IFrames(float time) {
        invincible = true;
        animator.SetTrigger("IFramesTrigger");
        while (time > 0) {
            time -= Time.deltaTime;
            yield return null;
        }
        invincible = false;
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 0) {
            Destroy(gameObject);
            return;
        }
        if (!dead) {
            transform.position = Vector3.zero;

            if (UIController.instance != null) {

                health_slider = UIController.instance.player_health_bar;

                ability1.slider = UIController.instance.ability1;
                ability2.slider = UIController.instance.ability2;
                ability3.slider = UIController.instance.ability3;
                ability4.slider = UIController.instance.ability4;

                UIController.instance.ability1_hover.display = ability1.display;
                UIController.instance.ability2_hover.display = ability2.display;
                UIController.instance.ability3_hover.display = ability3.display;
                UIController.instance.ability4_hover.display = ability4.display;
            }
            health = health;
        }
    }
}

public class Inventory {

    public int coins {
        get; private set;
    }
    public int boss_keys {
        get; private set;
    }
    public int keys {
        get; private set;
    }

    List<Item> _items;
    public ReadOnlyCollection<Item> items {
        get { return new ReadOnlyCollection<Item>(_items); }
    }

    GameObject inventory_holder;
    public Inventory(GameObject inventory_holder) {
        _items = new List<Item>();
        this.inventory_holder = inventory_holder;
    }

    public void PickUpKeys(int count = 1) {
        keys += count;
        UIController.instance.inventory.UpdateView(this);
    }

    public bool UseKey() {
        if (keys > 0) {
            keys--;
            UIController.instance.inventory.UpdateView(this);
            return true;
        } else {
            return false;
        }
    }

    public void PickUpBossKey() {
        boss_keys++;
        UIController.instance.inventory.UpdateView(this);
    }

    public bool UseBossKey() {
        if (boss_keys > 0) {
            boss_keys--;
            UIController.instance.inventory.UpdateView(this);
            return true;
        } else {
            return false;
        }
    }

    public void AddItemToInventory(Item i) {
        _items.Add(i);
        UIController.instance.ShowItemPopup(i);
        i.transform.SetParent(inventory_holder.transform);
    }
    public void RemoveItem(Item i) {
        i.Drop();
    }
}