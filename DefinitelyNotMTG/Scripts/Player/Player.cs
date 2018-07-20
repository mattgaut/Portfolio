using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable, ITargetable, ICombatant {

    [SerializeField]
    bool ai;
    [SerializeField]
    string button_to_pass;
    [SerializeField]
    List<Card> tests;
    [SerializeField]
    GameObject deck_object;
    List<Card> deck;

    [SerializeField]
    HandContainer _hand;
    public HandContainer hand {
        get { return _hand; }
    }
    [SerializeField]
    GraveyardContainer _graveyard;
    public GraveyardContainer graveyard {
        get { return _graveyard; }
    }
    [SerializeField]
    CardContainer _library;
    public CardContainer library {
        get { return _library; }
    }
    [SerializeField]
    FieldContainer _field;
    public FieldContainer field {
        get { return _field; }
    }

    [SerializeField]
    PlayerDisplay pd;
    [SerializeField]
    DisplayManaPool mana_pool_display;

    public ManaPool mana_pool {
        get; private set;
    }

    public int life {
        get; private set;
    }
    public int attack_power {
        get; private set;
    }
    public int health_remaining {
        get { return life; }
    }

    public PlayerAction current_action {
        get; private set;
    }

    public Player controller {
        get { return this; }
    }

    public void Start() {
        mana_pool = new ManaPool(mana_pool_display);
        life = 30;
        if (field != null) {
            foreach (Card test in tests) {
                field.AddCard(test);
                test.SetController(this);
                test.SetOwner(this);
            }
        }

        foreach (Card c in deck_object.GetComponentsInChildren<Card>()) {
            library.AddCard(c);
            c.SetController(this);
            c.SetOwner(this);
        }
    }

    public virtual void Update() {
        if (!ai && !GameManager.instance.paused) {
            if (Input.GetButtonDown(button_to_pass)) {
                PerformAction(new PassPriority());
            }
            if (Input.GetButtonDown("Confirm")) {
                PerformAction(new ConfirmAction());
            }
            if (Input.GetButtonDown("Cancel")) {
                PerformAction(new CancelAction());
            }
        }
    }

    public void PerformAction(PlayerAction pa) {
        StartCoroutine(PerformActionRoutine(pa));
    }

    IEnumerator PerformActionRoutine(PlayerAction pa) {
        current_action = pa;
        yield return null;
        yield return new WaitForEndOfFrame();
        if (current_action == pa) {
            current_action = null;
        }
    }

    public void UpdateDisplay() {
        pd.Display(this);
    }
    public void Highlight(bool is_highlighted) {
        pd.Highlight(is_highlighted);
    }

    public int TakeDamage(Card source, int damage) {
        life -= damage;
        return damage;
    }
    public int Heal(int amount) {
        life += amount;
        return amount;
    }

    public bool CanBeTargeted(ITargets targeter) {
        return true;
    }

    Coroutine check_click;
    void OnMouseEnter() {
        GameManager.instance.Hover(true, this);
        check_click = StartCoroutine(CheckClick());
    }
    void OnMouseExit() {
        if (check_click != null) {
            StopCoroutine(check_click);
            check_click = null;
        }
        GameManager.instance.Hover(false, this);
    }
    IEnumerator CheckClick() {
        while (true) {
            while (!Input.GetMouseButtonDown(0)) {
                yield return null;
            }
            while (!Input.GetMouseButtonUp(0)) {
                yield return null;
            }
            ContextClickHandler();
        }
    }
    void ContextClickHandler() {
        if (GameManager.instance.targeting && GameManager.instance.targeter.controller == GameManager.instance.player1) {
            GameManager.instance.ToggleTarget(this);
        } else if (GameManager.instance.attacking) {
            GameManager.instance.TryChooseAttackerTarget(this);
        }
        GameManager.instance.Hover(false, this);
        GameManager.instance.Hover(true, this);
    }
    
    public void TryPayMana(ManaType type) {
        PerformAction(new PayManaAction(this, type));
    }

    public void TryPayMana(int type) {
        PerformAction(new PayManaAction(this, (ManaType)type));
    }

    public bool CanBeAttacked(IAttacker attacker) {
        return true;
    }

    public int DealDamage(IDamageable com, int damage) {
        return 0;
    }
    public int DealCombatDamage(IDamageable com) {
        return 0;
    }
    public bool first_strike {
        get { return false; }
    }

    public bool CanPerformAction() {
        foreach (Spell c in hand.cards.OfType<Spell>()) {
            if (GameManager.instance.CardCanBePlayed(c) && HasMana(c)) {
                return true;
            }
        }
        foreach (PlayableCard c in hand.cards.OfType<PlayableCard>()) {
            if (GameManager.instance.CardCanBePlayed(c)) {
                return true;
            }
        }
        return false;
    }

    public bool HasMana(Spell c) {
        if (mana_pool.GetCurrent(ManaType.green) < c.mana_cost.green) {
            return false;
        }
        if (mana_pool.GetCurrent(ManaType.red) < c.mana_cost.red) {
            return false;
        }
        if (mana_pool.GetCurrent(ManaType.blue) < c.mana_cost.blue) {
            return false;
        }
        if (mana_pool.GetCurrent(ManaType.black) < c.mana_cost.black) {
            return false;
        }
        if (mana_pool.GetCurrent(ManaType.yellow) < c.mana_cost.yellow) {
            return false;
        }
        return c.mana_cost.yellow + c.mana_cost.green + c.mana_cost.red + c.mana_cost.black + c.mana_cost.blue + c.mana_cost.generic <=
            mana_pool.GetCurrent(ManaType.yellow) + mana_pool.GetCurrent(ManaType.green) + mana_pool.GetCurrent(ManaType.red) + mana_pool.GetCurrent(ManaType.black) + mana_pool.GetCurrent(ManaType.blue);
    }
}
