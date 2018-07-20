using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour {

    public static GameManager instance {
        get; private set;
    }

    [SerializeField]
    GameObject _targeting_object;
    public GameObject targeting_object {
        get { return _targeting_object; }
    }

    [SerializeField]
    TurnPanelController tpc;
    [SerializeField]
    StackContainer stack_container;
    [SerializeField]
    ZoomPanel zoom_panel;

    [SerializeField]
    GameObject win_screen;
    [SerializeField]
    Text win_text;
    [SerializeField]
    Text mana_pointer;

    [SerializeField]
    Player _player1, _player2;
    public Player player1 {
        get { return _player1; }
    }
    public Player player2 {
        get { return _player2; }
    }

    public Player current_turns_player {
        get; private set;
    }

    public Player active_player {
        get; private set;
    }

    public bool targeting {
        get; private set;
    }
    public bool attacking {
        get; private set;
    }
    public bool blocking {
        get; private set;
    }
    public bool ordering_blockers {
        get; private set;
    }
    public bool paying_mana {
        get; private set;
    }
    public bool paused {
        get; private set;
    }
    bool players_can_cast_spells {
        get { return !blocking && !attacking && !targeting; }
    }

    bool game_over;

    PlayerAction action_to_perform;

    List<IStackEffect> stack;
    Dictionary<IStackEffect, GameObject> stack_targeting;

    public FightManager fight_manager {
        get; private set;
    }
    public TriggerManager trigger_manager {
        get; private set;
    }
    public StaticAbilityManager static_ability_manager {
        get; private set;
    }

    public Phase current_phase {
        get; private set;
    }

    Dictionary<Player, int> cards_drawn_this_turn;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        stack = new List<IStackEffect>();
        fight_manager = new FightManager();
        trigger_manager = new TriggerManager();
        static_ability_manager = new StaticAbilityManager();
        stack_targeting = new Dictionary<IStackEffect, GameObject>();
        highlighted_attacker_targets = new List<ICombatant>();
        hover_targeting_display = Instantiate(targeting_object).GetComponent<TargetingDisplay>();

        cards_drawn_this_turn = new Dictionary<Player, int>();
        cards_drawn_this_turn.Add(player1,0);
        cards_drawn_this_turn.Add(player2, 0);
    }

    void Start() {
        StartCoroutine(GameLoop());
    }

    bool player_1_auto_pass;
    public void TogglePlayer1AutoPassPriority(Toggle t) {
        player_1_auto_pass = t.isOn;
    }

    public void Pause(bool pause) {
        paused = pause;
    }

    IEnumerator GameLoop() {
        current_turns_player = Random.Range(0, 1) == 0 ? player1 : player2;
        yield return PreGame();
        while (!game_over) {
            tpc.SetTurn(current_turns_player == player1);
            yield return PlayTurn();
            SwitchCurrentTurnsPlayer();
        }
    }

    IEnumerator PreGame() {
        yield return null;
        player1.library.Shuffle();
        yield return DrawCard(player1, 7);
        cards_drawn_this_turn[player1] = 0;
    }

    IEnumerator PlayTurn() {
        active_player = current_turns_player;
        // Start
        current_phase = Phase.start;
        tpc.SetPhase(current_phase);
        yield return Upkeep();
        yield return ResolveStack();

        // Main 1
        current_phase = Phase.main1;
        tpc.SetPhase(current_phase);
        yield return ResolveStack();


        // Declare Attackers
        fight_manager.Clear();
        current_phase = Phase.attackers;
        tpc.SetPhase(current_phase);
        yield return DeclareAttackers();
        yield return ResolveStack();

        // Blockers
        if (fight_manager.HasFights()) { //if attackers are declared
            current_phase = Phase.blockers;
            tpc.SetPhase(current_phase);
            yield return DeclareBlockers();
            fight_manager.SetBlocked();
            yield return ResolveStack();

            // Damage
            current_phase = Phase.damage;
            tpc.SetPhase(current_phase);
            if (ApplyFirstStrikeDamage())
                yield return ResolveStack();
            ApplyFightDamage();
            fight_manager.Clear();
            yield return ResolveStack();
        }

        // Main2
        current_phase = Phase.main2;
        tpc.SetPhase(current_phase);
        yield return ResolveStack();

        // End
        current_phase = Phase.end;
        tpc.SetPhase(current_phase);
        yield return EndOfTurn();
        yield return ResolveStack();

        yield return Cleanup();
    }

    IEnumerator Upkeep() {
        has_played_mana = false;
        current_turns_player.mana_pool.Fill();
        foreach (Card c in current_turns_player.field.cards) {
            c.BeginTurn();
        }
        yield return DrawCard(current_turns_player, 1);
    }
    IEnumerator DeclareAttackers() {
        attacking = true;
        active_player = current_turns_player;
        tpc.SetWaiting("Declare Attackers");
        if (player1 == current_turns_player) {
            tpc.SetButtonSet(ButtonSet.confirm);
            HighlightPossibleAttackers();
        }
        if ((current_turns_player.field.cards.OfType<IAttacker>().Where((IAttacker c) => { return c.CanAttack(); })).Count() > 0) {           
            PlayerAction player_action = null;
            while (!(player_action as ConfirmAction != null) ||  declaring_attacker) {
                yield return WaitForPlayerAction(current_turns_player, (PlayerAction action) => player_action = action, false);
                if ((player_action as CancelAction != null) && declaring_attacker) {
                    EndAttackerTargeting();
                }
            }
            ClearAttackerHighlights();
            foreach (IAttacker attacker in fight_manager.attackers_to_fights.Keys) {
                attacker.card.ExhaustUnit(true);
            }
        }
        tpc.SetWaiting("");
        if (player1 == current_turns_player) tpc.SetButtonSet(ButtonSet.none);
        attacking = false;
        foreach (IAttacker c in fight_manager.attackers_to_fights.Keys) {
            foreach (TriggeredAbility ta in c.card.triggers.GetLocalTriggers(TriggerType.attacks)) {
                yield return ta.OnTrigger();
            }
        }
    }
    IEnumerator DeclareBlockers() {
        blocking = true;
        active_player = OtherPlayer(current_turns_player);
        if (player1 == active_player) tpc.SetButtonSet(ButtonSet.confirm);
        tpc.SetWaiting("Declare Blockers");
        yield return null;
        if (OtherPlayer(current_turns_player).field.HasCreatures()) {
            if (player2 == current_turns_player) HighlightPossibleBlockers();
            while (!(active_player.current_action as ConfirmAction != null) || declaring_blocker) {
                yield return WaitForPlayerAction(active_player, set_buttons: false);
                if ((active_player.current_action as CancelAction != null) && declaring_blocker) {

                    EndBlockerTargeting();
                }
            }
            ClearBlockerHighlights();
        }
        if (player1 == active_player) tpc.SetButtonSet(ButtonSet.none);
        tpc.SetWaiting("");
        active_player = current_turns_player;
        blocking = false;

        yield return OrderBlockers();
    }
    IEnumerator OrderBlockers() {
        ordering_blockers = true;
        tpc.SetWaiting("Choose the order blockers will be dealt damage.");
        foreach (Fight f in fight_manager.fights) {
            if (f.blockers.Count > 1) {
                StartOrdering(f);
                tpc.SetButtonSet(ButtonSet.confirm);
                while (!(active_player.current_action as ConfirmAction != null) || ordered_blockers.Count < f.blockers.Count) {
                    yield return null;
                    tpc.ConfirmButtonInteractable(ordered_blockers.Count == f.blockers.Count);
                }
                EndOrdering();
            }
        }
        ordering_blockers = false;
    }

    bool ApplyFirstStrikeDamage() {
        List<Fight> fights = new List<Fight>(fight_manager.fights);
        bool damage_applied = false;
        for (int i = 0; i < fights.Count; i++) {
            if (fights[i].attacker_was_blocked) {
                int damage_to_split = fights[i].attacker.attack_power;
                List<IBlocker> blockers = new List<IBlocker>(fights[i].GetOrderedBlockers());
                for (int j = 0; j < blockers.Count; j++) {
                    if (blockers[j].first_strike) {
                        blockers[j].DealCombatDamage(fights[i].attacker);
                        damage_applied = true;
                    }
                }
                if (fights[i].attacker.first_strike) {
                    for (int j = 0; j < blockers.Count; j++) {
                        int damage_to_apply = blockers[j].health_remaining < damage_to_split ? blockers[j].health_remaining : damage_to_split;
                        fights[i].attacker.DealDamage(blockers[j], damage_to_apply > 0 ? damage_to_apply : 0);
                        damage_to_split -= damage_to_apply;
                    }
                    damage_applied = true;
                }
            } else {
                if (fights[i].attacked != null) {
                    if (fights[i].attacked.first_strike) {
                        damage_applied = true;
                        fights[i].attacked.DealCombatDamage(fights[i].attacker);
                    }
                    if (fights[i].attacker.first_strike) {
                        damage_applied = true;
                        fights[i].attacker.DealCombatDamage(fights[i].attacked);
                    }
                }
            }
        }
        return damage_applied;
    }
    void ApplyFightDamage() {
        List<Fight> fights = new List<Fight>(fight_manager.fights);
        for (int i = 0; i < fights.Count; i++) {
            if (fights[i].attacker_was_blocked) {
                int damage_to_split = fights[i].attacker.attack_power;
                List<IBlocker> blockers = new List<IBlocker>(fights[i].GetOrderedBlockers());
                for (int j = 0; j < blockers.Count; j++) {
                    if (!blockers[j].first_strike) {
                        blockers[j].DealCombatDamage(fights[i].attacker);
                    }
                }
                if (!fights[i].attacker.first_strike) {
                    for (int j = 0; j < blockers.Count; j++) {
                        int damage_to_apply = blockers[j].health_remaining < damage_to_split ? blockers[j].health_remaining : damage_to_split;
                        fights[i].attacker.DealDamage(blockers[j], damage_to_apply > 0 ? damage_to_apply : 0);
                        damage_to_split -= damage_to_apply;
                    }
                }
            } else {
                if (fights[i].attacked != null) {
                    if (!fights[i].attacked.first_strike)
                        fights[i].attacked.DealCombatDamage(fights[i].attacker);
                    if (!fights[i].attacker.first_strike)
                        fights[i].attacker.DealCombatDamage(fights[i].attacked);
                }
            }
        }
    }
    IEnumerator EndOfTurn() {
        yield return trigger_manager.Trigger(new EOTTriggerInfo(current_turns_player));
    }
    IEnumerator Cleanup() {
        yield return null;

        foreach (Card c in player1.field.cards) {
            c.EOTCleanup();
        }
        foreach (Card c in player2.field.cards) {
            c.EOTCleanup();
        }
        cards_drawn_this_turn[player1] = 0;
        cards_drawn_this_turn[player2] = 0;
    }

    public Player waiting_for {
        get; private set;
    }
    IEnumerator WaitForPlayerAction(Player wait_for, System.Action<PlayerAction> callback = null, bool set_buttons = true) {
        waiting_for = wait_for;

        if (active_player == player1 && set_buttons) tpc.SetButtonSet(ButtonSet.pass);

        while (wait_for.current_action == null || (waiting_for == player1 && paused)) {
            yield return null;
        }

        if (active_player == player1 && set_buttons) tpc.SetButtonSet(ButtonSet.none);
        action_to_perform = wait_for.current_action;
        if (callback != null) {
            callback(action_to_perform);
        }

        waiting_for = null;
    }

    public IEnumerator AddToStack(IStackEffect effect) {
        Card c = effect as Card;

        if (c != null) {
            static_ability_manager.RemoveFromStaticAbilities(c.zone, c);
        }

        stack.Add(effect);
        stack_container.Add(effect);

        if (c != null) {
            static_ability_manager.AddToStaticAbilities(c.zone, c);
        }

        yield return effect.OnEnterStack();
    }

    IStackEffect PopFromStack() {
        if (stack.Count < 1)
            return null;

        IStackEffect ret = stack[stack.Count - 1];
        stack.RemoveAt(stack.Count - 1);
        stack_container.Pop();
        return ret;
    }

    IEnumerator ResolveStack() {
        int stack_before;
        do {
            tpc.SetWaiting(active_player == player1);
            bool passed = false;
            do {

                yield return CheckStateBasedEffects();

                stack_before = stack.Count;

                PlayerAction action = null;
                if (player1 == active_player && player_1_auto_pass && !player1.CanPerformAction()) {
                    passed = true;
                    yield return null;
                } else {
                    yield return WaitForPlayerAction(active_player, (PlayerAction a) => action = a);
                    passed = typeof(PassPriority) == action.GetType();
                    yield return action.PerformAction();
                }


                if (stack_before != stack.Count) { // replace with better method to find if player cast spell or activated ability
                    continue;
                }
            } while (!passed);
            SwitchActivePlayer();

            tpc.SetWaiting(active_player == player1);

            passed = false;

            do {

                yield return CheckStateBasedEffects();

                stack_before = stack.Count;

                PlayerAction action = null;
                if (player1 == active_player && player_1_auto_pass && !player1.CanPerformAction()) {
                    passed = true;
                    yield return null;
                } else {
                    yield return WaitForPlayerAction(active_player, (PlayerAction a) => action = a);
                    passed = typeof(PassPriority) == action.GetType();
                    yield return action.PerformAction();
                }

                if (stack_before != stack.Count) { // replace with better method to find if player cast spell or activated ability
                    break;
                }
            } while (!passed);
            if (stack_before != stack.Count) { // replace with better method to find if player cast spell or activated ability
                continue;
            }
            SwitchActivePlayer();

            if (stack.Count > 0) {
                IStackEffect effect = PopFromStack();
                yield return effect.OnResolution();
                yield return effect.OnLeaveStack();
                yield return CheckStateBasedEffects();
            }
            SwitchActivePlayer(current_turns_player);
        } while (stack.Count > 0 || stack_before != 0);
        yield return null;
    }

    void SwitchActivePlayer() {
        if (active_player == player1) {
            active_player = player2;
        } else {
            active_player = player1;
        }
    }
    void SwitchActivePlayer(Player p) {
        active_player = p;
    }

    void SwitchCurrentTurnsPlayer() {
        if (current_turns_player == player1) {
            current_turns_player = player2;
        } else {
            current_turns_player = player1;
        }
    }
    public Player OtherPlayer(Player p) {
        if (p == player1) {
            return player2;
        } else {
            return player1;
        }
    }

    bool has_played_mana;
    public bool CardCanBePlayed(Card c, bool error_message = false) {
        if (c.GetType() == typeof(Creature) || c.GetType() == (typeof(Structure))) {
            if (c.controller.field.cards.Count >= 7) {
                if (error_message && active_player == player1) UIMessage.instance.DisplayMessage("Field is full.");
                return false;
            }
        }
        if (c.GetType() == typeof(Mana)) {
            if (!CanBePlayedInCurrentPhase(c)) {
                if (error_message && active_player == player1) UIMessage.instance.DisplayMessage("Card must be played in one of your main phases.");
                return false;
            }
            if (!CanBePlayedOnCurrentStack(c)) {
                if (error_message && active_player == player1) UIMessage.instance.DisplayMessage("Card cannot be played at instant speed.");
                return false;
            }
            if (has_played_mana) {
                if (error_message && active_player == player1) UIMessage.instance.DisplayMessage("Already played mana this turn.");
                return false;
            }
            return true;
        } else {
            if (!CanBePlayedInCurrentPhase(c)) {
                if (error_message && active_player == player1)
                    UIMessage.instance.DisplayMessage("Card must be played in one of your main phases.");
                return false;
            }
            if (!CanBePlayedOnCurrentStack(c)) {
                if (error_message && active_player == player1)
                    UIMessage.instance.DisplayMessage("Card cannot be played at instant speed.");
                return false;
            }
            if (active_player == player1 && !active_player.HasMana(c as Spell)) {
                if (error_message && active_player == player1)
                    UIMessage.instance.DisplayMessage("Not Enough Mana.");
                return false;
            }
            return true;
        }
    }
    public bool CanBePlayedInCurrentPhase(Card c) {
        if (c.major_card_type == MajorCardType.instant) {
            return true;
        } else {
            return (current_phase == Phase.main1 || current_phase == Phase.main2) && c.controller == current_turns_player;
        }
    }
    public bool CanBePlayedOnCurrentStack(Card c) {
        if (c.major_card_type == MajorCardType.instant) {
            return true;
        } else {
            return stack.Count == 0;
        }
    }

    public void HighlightSelectable(SelectionType selection, bool is_highlighted) {
        if (attacking) {
            if (is_highlighted)
                HighlightPossibleAttakerTargets();
            else
                ClearAttackerHighlights(); 
        }
    }

    public List<Card> GetCardsInZone(Zone z, Player p = null) {
        List<Card> ret = new List<Card>();
        if (z == Zone.stack) {
            ret.AddRange(stack_container.cards);
        } else if (z == Zone.field) {
            if (p == null || player1 == p) {
                ret.AddRange(player1.field.cards);
            }
            if (p == null || player2 == p) {
                ret.AddRange(player2.field.cards);
            }
        } else if (z == Zone.hand) {
            if (p == null || player1 == p) {
                ret.AddRange(player1.hand.cards);
            }
            if (p == null || player2 == p) {
                ret.AddRange(player2.hand.cards);
            }
        } else if (z == Zone.graveyard) {
            if (p == null || player1 == p) {
                ret.AddRange(player1.graveyard.cards);
            }
            if (p == null || player2 == p) {
                ret.AddRange(player2.graveyard.cards);
            }
        } else if (z == Zone.library) {
            if (p == null || player1 == p) {
                ret.AddRange(player1.library.cards);
            }
            if (p == null || player2 == p) {
                ret.AddRange(player2.library.cards);
            }
        } if (z == Zone.stack) {
            ret.AddRange(stack_container.cards);
        }
        return ret;
    }

    ////////////
    // Targeting
    ////////////
    List<ITargetable> targets;
    public ITargets targeter {
        get; private set;
    }
    TargetingDisplay targeting_display;
    List<ITargetable> highlighted_targets;
    public int min_targets {
        get; private set;
    }
    public int max_targets {
        get; private set;
    }
    Player active_before;
    public List<ITargetable> StartTargeting(ITargets targeter, bool cancelable, int min, int max, Card display_card, string message = "Choose Target(s)") {
        if (targeting) {
            throw new System.Exception("Already in targeting routine");
        } else {
            active_before = active_player;
            active_player = player1;
            if (display_card) {
                Zoom(display_card, false);
            }
            highlighted_targets = new List<ITargetable>();
            targets = new List<ITargetable>();
            min_targets = min;
            max_targets = max;
            targeting_display = Instantiate(targeting_object).GetComponent<TargetingDisplay>();
            //targeting_display.SetForceActive(true);
            tpc.SetButtonSet(cancelable ? ButtonSet.confirm_cancel : ButtonSet.confirm);
            tpc.SetWaiting(message);
            tpc.ConfirmButtonInteractable(min <= targets.Count);
            this.targeter = targeter;
            targeting = true;
            StartCoroutine(WaitForTargets());
            return targets;
        }
    }
    IEnumerator WaitForTargets() {
        yield return null;
        HighlightPossibleTargets();
    }
    public void EndTargeting() {
        active_player = active_before;
        ClearZoom(false);
        targeter = null;
        targeting = false;
        targets = null;
        tpc.SetWaiting("");
        targeting_display.Clear();
        targeting_display = null;
        ClearTargetingHighlights();
        tpc.SetButtonSet(ButtonSet.none);
    }

    public void AddTarget(ITargetable target) {
        if (targeting && target.CanBeTargeted(targeter) && targeter.CanTarget(target)) {
            targets.Add(target);
            targeting_display.AddTarget(target.gameObject);
            ClearTargetingHighlights();
            HighlightPossibleTargets();
            tpc.ConfirmButtonInteractable(min_targets <= targets.Count);
        }
    }
    public void RemoveTarget(ITargetable target) {
        if (targeting) {
            targets.Remove(target);
            targeting_display.RemoveTarget(target.gameObject);
            ClearTargetingHighlights();
            HighlightPossibleTargets();
            tpc.ConfirmButtonInteractable(min_targets <= targets.Count);
        }
    }
    public void ToggleTarget(ITargetable target) {
        if (targeting) {
            if (targets.Contains(target)) {
                RemoveTarget(target);
            } else {
                AddTarget(target);
            }
        }
    }
    public List<ITargetable> GetPossibleTargets() {
        return GetPossibleTargets(targeter);
    }
    public List<ITargetable> GetPossibleTargets(ITargets possible_targeter) {
        List<ITargetable> ret = new List<ITargetable>();
        foreach (ITargetable com in player1.field.cards) {
            if (com.CanBeTargeted(possible_targeter) && possible_targeter.TrueCanTarget(com)) {
                ret.Add(com);
            }
        }
        foreach (ITargetable com in player2.field.cards) {
            if (com.CanBeTargeted(possible_targeter) && possible_targeter.TrueCanTarget(com)) {
                ret.Add(com);
            }
        }
        foreach (ITargetable com in stack) {
            if (com.CanBeTargeted(possible_targeter) && possible_targeter.TrueCanTarget(com)) {
                ret.Add(com);
            }
        }
        if (player1.CanBeTargeted(possible_targeter) && possible_targeter.TrueCanTarget(player1)) {
            ret.Add(player1);
        }
        if (player2.CanBeTargeted(possible_targeter) && possible_targeter.TrueCanTarget(player2)) {
            ret.Add(player2);
        }
        return ret;
    }
    void HighlightPossibleTargets() {
        if (targeting) {
            foreach (ITargetable com in GetPossibleTargets()) {
                com.Highlight(true);
                highlighted_targets.Add(com);
            }
        }
    }
    void ClearTargetingHighlights() {
        foreach (ITargetable com in highlighted_targets) {
            com.Highlight(false);
        }
        highlighted_targets.Clear();
    }
    ////////////
    ////////////
    ////////////

    ////////////
    // Attacking
    ////////////
    IAttacker new_attacker;
    ICombatant new_attacker_target;
    bool declaring_attacker;
    List<ICombatant> highlighted_attacker_targets;
    public void StartAttackerTargeting(IAttacker attacker) {
        if (fight_manager.attackers_to_fights.ContainsKey(attacker)) {
            if (player1 == active_player) tpc.SetButtonSet(ButtonSet.confirm);
            fight_manager.RemoveFight(attacker);
            HighlightPossibleAttakerTargets();
            tpc.SetWaiting("Declare Attackers");
        } else {
            if (attacker.CanAttack()) {
                if (player1 == active_player) tpc.SetButtonSet(ButtonSet.cancel);
                new_attacker = attacker;
                declaring_attacker = true;
                new_attacker_target = null;
                HighlightPossibleAttakerTargets();
                tpc.SetWaiting("Choose Target to Attack");
            } else {
                if (attacker.card.triggers.keywords.defender) {
                    UIMessage.instance.DisplayMessage("Creatures with defender cannot attack.");
                } else if (attacker.card.summoning_sick) {
                    UIMessage.instance.DisplayMessage("Summoning Sick creatures cannot attack.");
                } else {
                    UIMessage.instance.DisplayMessage("Creature cannot attack.");
                }
            }
        }
    }
    public void EndAttackerTargeting() {
        if (new_attacker_target != null && new_attacker != null) {
            fight_manager.NewFight(new_attacker, new_attacker_target);
        }
        new_attacker_target = null;
        new_attacker = null;
        tpc.SetWaiting("Declare Attackers");
        if (player1 == active_player) tpc.SetButtonSet(ButtonSet.confirm);
        declaring_attacker = false;
        ClearAttackerHighlights();
        HighlightPossibleAttackers();
    }
    public void TryChooseAttackerTarget(ICombatant attacked) {
        if (declaring_attacker && new_attacker.CanAttack(attacked) && attacked.CanBeAttacked(new_attacker)) {
            new_attacker_target = attacked;
            EndAttackerTargeting();
        } else {
            if (attacked as IBlocker != null &&  (attacked as IBlocker).card.triggers.keywords.flying) {
                UIMessage.instance.DisplayMessage("Flying creatures can only be attacked or blocked by other flying creatures.");
            } else {
                UIMessage.instance.DisplayMessage("Creature cannot attack target");
            }
        }
    }
    void HighlightPossibleAttackers() {
        ClearAttackerHighlights();
        foreach (IAttacker com in current_turns_player.field.cards.OfType<IAttacker>()) {
            if (com.CanAttack() && !fight_manager.attackers_to_fights.ContainsKey(com)) {
                com.Highlight(true);
                highlighted_attacker_targets.Add(com);
            }
        }
    }
    void HighlightPossibleAttakerTargets() {
        if (declaring_attacker) {
            ClearAttackerHighlights();
            foreach (ICombatant com in OtherPlayer(current_turns_player).field.cards) {
                if (com.CanBeAttacked(new_attacker) && new_attacker.CanAttack(com)) {
                    com.Highlight(true);
                    highlighted_attacker_targets.Add(com);
                }
            }
            Player p = OtherPlayer(current_turns_player);
            if (p.CanBeAttacked(new_attacker) && new_attacker.CanAttack(p)) {
                p.Highlight(true);
                highlighted_attacker_targets.Add(p);
            }
        }
    }
    void ClearAttackerHighlights() {
        foreach (ICombatant com in highlighted_attacker_targets) {
            com.Highlight(false);
        }
        highlighted_attacker_targets.Clear();
    }
    ////////////
    ////////////
    ////////////

    ////////////
    // Blocking
    ////////////
    IBlocker new_blocker;
    IAttacker new_blocker_target;
    bool declaring_blocker;
    public void StartBlockerTargeting(IBlocker blocker) {
        if (!fight_manager.BeingAttacked(blocker) && blocker.CanBlock()) {
            if (fight_manager.AssignedAsBlocker(blocker)) {
                fight_manager.RemoveBlocker(blocker);
                tpc.SetWaiting("Declare Blockers");
                HighlightPossibleBlockers();
                if (player1 == active_player) tpc.SetButtonSet(ButtonSet.confirm);
            } else {
                new_blocker = blocker;
                tpc.SetWaiting("Choose Attacker to Block");
                ClearBlockerHighlights();              
                if (player1 == active_player) tpc.SetButtonSet(ButtonSet.cancel);
                declaring_blocker = true;
                new_blocker_target = null;
                HighlightPossibleBlockerTargets();
            }
        } else if (!blocker.CanBlock()) {
            if (blocker.card.exhausted) {
                UIMessage.instance.DisplayMessage("Exhausted creatures cannot block.");
            } else {
                UIMessage.instance.DisplayMessage("Creature cannot block.");
            }
        } else {
            UIMessage.instance.DisplayMessage("Creatures being attacked cannot block.");
        }
    }
    public void EndBlockerTargeting() {
        if (new_blocker_target != null && new_blocker != null) {
            fight_manager.DeclareBlocker(new_blocker, new_blocker_target);
        }
        new_blocker = null;
        new_attacker_target = null;
        HighlightPossibleBlockers();
        if (player1 == active_player) tpc.SetButtonSet(ButtonSet.confirm);
        declaring_blocker = false;
    }
    public void TryChooseBlockerTarget(IAttacker blocked) {
        if (new_blocker != null) {
            if (!fight_manager.attackers_to_fights.ContainsKey(blocked)) {
                UIMessage.instance.DisplayMessage("Target not attacking");
                return;
            }
            if (declaring_blocker && new_blocker.CanBlock(blocked) && blocked.CanBeBlocked(new_blocker)) {
                new_blocker_target = blocked;
                EndBlockerTargeting();
            } else {
                if (!fight_manager.attackers_to_fights.ContainsKey(blocked)) {
                    UIMessage.instance.DisplayMessage("Target not attacking");
                } else if (!new_blocker.card.triggers.keywords.flying && blocked.card.triggers.keywords.flying) {
                    UIMessage.instance.DisplayMessage("Flying creatures can only be attacked or blocked by other flying creatures.");
                } else {
                    UIMessage.instance.DisplayMessage("Creature cannot block this Attacker.");
                }
            }
        }
    }
    void HighlightPossibleBlockers() {
        ClearBlockerHighlights();
        foreach (IBlocker com in OtherPlayer(current_turns_player).field.cards.OfType<IBlocker>()) {
            if (com.CanBlock() && !fight_manager.AssignedAsBlocker(com) && !fight_manager.BeingAttacked(com)) {
                    com.Highlight(true);
                    highlighted_attacker_targets.Add(com);
            }
        }
    }
    void HighlightPossibleBlockerTargets() {
        if (declaring_blocker) {
            ClearBlockerHighlights();
            foreach (IAttacker com in current_turns_player.field.cards.OfType<IAttacker>()) {
                if (fight_manager.attackers_to_fights.ContainsKey(com) && com.CanBeBlocked(new_blocker) && new_blocker.CanBlock(com)) {
                    com.Highlight(true);
                    highlighted_attacker_targets.Add(com);
                }
            }
        }
    }
    void ClearBlockerHighlights() {
        foreach (ICombatant com in highlighted_attacker_targets) {
            com.Highlight(false);
        }
        highlighted_attacker_targets.Clear();
    }
    ////////////
    ////////////
    ////////////

    ////////////
    // Order
    ////////////
    public Fight ordering_for {
        get; private set;
    }
    List<IBlocker> ordered_blockers;
    void StartOrdering(Fight ordering_for) {
        this.ordering_for = ordering_for;
        ordered_blockers = new List<IBlocker>();
        foreach (IBlocker b in ordering_for.blockers) {
            b.Highlight(true);
        }
    }
    public void TryAddOrderTarget(IBlocker b) {
        if (ordering_for.blockers.Contains(b)) {
            if (ordered_blockers.Contains(b)) {
                ordered_blockers.Remove(b);
                b.Highlight(true);
            } else {
                ordered_blockers.Add(b);
                b.Highlight(false);
            }
            ordering_for.SetOrderedBlockers(ordered_blockers);
        }
    }
    void EndOrdering() {
        foreach (IBlocker b in ordering_for.blockers) {
            b.Highlight(false);
        }
    }
    ////////////
    ////////////
    ////////////

    public IEnumerator MoveToField(SpellPermanent permanent) {
        static_ability_manager.RemoveFromStaticAbilities(permanent.zone, permanent);
        permanent.contained_in.RemoveCard(permanent);
        permanent.controller.field.AddCard(permanent);

        permanent.triggers.Subscribe();

        static_ability_manager.AddToStaticAbilities(permanent.zone, permanent);
        foreach (TriggeredAbility ta in permanent.triggers.GetLocalTriggers(TriggerType.etb)) {
            yield return ta.OnTrigger();
        }
        yield return trigger_manager.Trigger(new ETBTriggerInfo(permanent));
    }
    public IEnumerator DestroyPermanent(Card permanent) {
        if (permanent.zone == Zone.field) {
            if (permanent as ICombatant != null) {
                fight_manager.RemoveCombatantFromAllFights((ICombatant)permanent);
            }
            yield return MoveToGraveyard(permanent);
        }
    }
    public IEnumerator MoveToGraveyard(Card card_to_move) {
        static_ability_manager.RemoveFromStaticAbilities(card_to_move.zone, card_to_move);
        Zone previous_zone = card_to_move.zone;
        card_to_move.owner.graveyard.AddCard(card_to_move);

        static_ability_manager.AddToStaticAbilities(card_to_move.zone, card_to_move);
        if (previous_zone == Zone.field) {
            card_to_move.triggers.UnSubscribe();
            foreach (TriggeredAbility ta in card_to_move.triggers.GetLocalTriggers(TriggerType.dies)) {
                yield return ta.OnTrigger();
            }
            yield return trigger_manager.Trigger(new DiesTriggerInfo(card_to_move));
        }
    }
    public IEnumerator MoveToLibrary(Card card_to_move) {
        static_ability_manager.RemoveFromStaticAbilities(card_to_move.zone, card_to_move);
        card_to_move.owner.library.AddCard(card_to_move);
        static_ability_manager.AddToStaticAbilities(card_to_move.zone, card_to_move);
        yield return null;
    }
    public IEnumerator MoveToHand(Card card_to_move) {
        static_ability_manager.RemoveFromStaticAbilities(card_to_move.zone, card_to_move);
        card_to_move.owner.hand.AddCard(card_to_move);
        static_ability_manager.AddToStaticAbilities(card_to_move.zone, card_to_move);
        yield return null;
    }

    public IEnumerator DrawCard(Player p, int count) {
        for (int i = 0; i < count; i++) {
            if (p.library.cards.Count > 0) {
                Card c = p.library.cards[0];
                yield return MoveToHand(c);
                cards_drawn_this_turn[p] += 1;
                yield return trigger_manager.Trigger(new DrawTriggerInfo(c, cards_drawn_this_turn[p]));
            }
        }
    }

    public IEnumerator CheckStateBasedEffects() {
        bool repeat;

        do {
            List<Card> cards_on_field = new List<Card>(player1.field.cards);
            cards_on_field.AddRange(player2.field.cards);

            repeat = false;
            player1.UpdateDisplay();
            player2.UpdateDisplay();
            if (player1.life <= 0) {
                if (player2.life <= 0) {
                    EndGame();
                    while (true) { yield return null; }
                } else {
                    EndGame(player2);
                    while (true) { yield return null; }
                }
            }
            if (player2.life <= 0) {
                EndGame(player1);
                while (true) { yield return null; }
            }
            List<SpellCombatant> to_die = new List<SpellCombatant>();
            foreach (SpellCombatant c in player1.field.cards.OfType<SpellCombatant>()) {
                c.UpdateDisplay();
                if (c.dead) {
                    to_die.Add(c);
                    repeat = true;
                }
            }
            foreach (SpellCombatant c in player2.field.cards.OfType<SpellCombatant>()) {
                c.UpdateDisplay();
                if (c.dead) {
                    to_die.Add(c);
                    repeat = true;
                }
            }
            foreach (SpellCombatant c in to_die) {
                fight_manager.RemoveCombatantFromAllFights(c);
                yield return MoveToGraveyard(c);
            }
            foreach (SpellCombatant c in to_die) {
                c.triggers.UnSubscribe();
            }
        } while (repeat);
    }
    public void EndGame(Player winner = null) {
        win_screen.SetActive(true);
        if (winner == null) {
            win_text.text = "Draw!";
        } else {
            if (winner == player1) {
                win_text.text = "You Win!";
            } else {
                win_text.text = "You Lose!";
            }
        }
    }

    TargetingDisplay hover_targeting_display;
    public void Hover(bool is_hovering, Card c) {
        if (is_hovering == false) {
            hover_targeting_display.Clear(false);
        }
        if (targeting && is_hovering) {
            if (targeter.controller == player1 && targeter.CanTarget(c) && c.CanBeTargeted(targeter)) {
                hover_targeting_display.AddTarget(c.gameObject);
            }
        } else if(current_turns_player != player1 && declaring_blocker && is_hovering) {
            IAttacker com = c as IAttacker;
            if (com != null && new_blocker.CanBlock(com) && com.CanBeBlocked(new_blocker) && fight_manager.attackers_to_fights.ContainsKey(com)) {
                hover_targeting_display.AddTarget(c.gameObject);
            }
        } else if (current_turns_player == player1 && declaring_attacker && is_hovering) {
            ICombatant com = c as ICombatant;
            if (com != null && new_attacker.CanAttack(com) && com.CanBeAttacked(new_attacker)) {
                hover_targeting_display.AddTarget(c.gameObject);
            }
        } else {
            if (c.zone == Zone.field) {
                if (c.controller == current_turns_player) {
                    IAttacker com = c as IAttacker;
                    if (com != null) {
                        List<Fight> fights = fight_manager.InvolvedInFights(com, true, false, false);
                        foreach (Fight f in fights)
                            f.HighlightFight(is_hovering);
                    }
                } else {
                    IBlocker block = c as IBlocker;
                    if (block != null) {
                        List<Fight> fights = fight_manager.BlockerInFights(block);
                        foreach (Fight f in fights)
                            f.HighlightFight(is_hovering);
                    }
                }
            } else if (c.zone == Zone.stack) {
                IStackEffect stack_effect = c as IStackEffect;
                if (stack_effect != null)
                    stack_container.DisplayTargetingIcons(stack_effect, is_hovering);
            }
        }

    }
    public void Hover(bool is_hovering, TriggeredAbility.TriggerInstance c) {
        if (targeting && targeter.controller == player1) {
            if (targeter.CanTarget(c) && c.CanBeTargeted(targeter)) {
                if (is_hovering) {
                    hover_targeting_display.AddTarget(c.gameObject);
                }
            }
        } else {
            if (is_hovering == false) {
                hover_targeting_display.Clear(false);
            }
            stack_container.DisplayTargetingIcons(c, is_hovering);
        }
    }
    public void Hover(bool is_hovering, Player p) {
        if (is_hovering == false) {
            hover_targeting_display.Clear(false);
        }
        if (current_turns_player == player1 && declaring_attacker && is_hovering) {
            if (new_attacker.CanAttack(p) && p.CanBeAttacked(new_attacker)) {
                hover_targeting_display.AddTarget(p.gameObject);
            }
        } else if (targeting) {
            if (targeter.controller == player1 && targeter.CanTarget(p) && p.CanBeTargeted(targeter)) {
                if (is_hovering) {
                    hover_targeting_display.AddTarget(p.gameObject);
                }
            }
        } else {
            ICombatant com = p as ICombatant;
            if (com != null) {
                List<Fight> fights = fight_manager.InvolvedInFights(com, false, false, true);
                foreach (Fight f in fights)
                    f.HighlightFight(is_hovering);
            }
        }
    }
    public void Zoom(Card display, bool is_active = true) {
        if (is_active) {
            zoom_panel.DisplayActive(display);
        } else {
            zoom_panel.DisplayPassive(display);
        }
    }
    public void ClearZoom(bool is_active = true) {
        if (is_active) {
            zoom_panel.ClearActive();
        } else {
            zoom_panel.ClearPassive();
        }
    }


    bool first_time = true;
    public IEnumerator PlayerPayMana(ManaCost cost, Player p, System.Action<bool, ManaPool> callback) {
        bool paid_for = false;
        ManaPool paid_mana = new ManaPool();

        if (first_time) {
            mana_pointer.enabled = true;
        }

        paid_mana.AddCurrent(ManaType.black, p.mana_pool.SubtractCurrent(ManaType.black, cost.black));
        paid_mana.AddCurrent(ManaType.blue, p.mana_pool.SubtractCurrent(ManaType.blue, cost.blue));
        paid_mana.AddCurrent(ManaType.green, p.mana_pool.SubtractCurrent(ManaType.green, cost.green));
        paid_mana.AddCurrent(ManaType.yellow, p.mana_pool.SubtractCurrent(ManaType.yellow, cost.yellow));
        paid_mana.AddCurrent(ManaType.red, p.mana_pool.SubtractCurrent(ManaType.red, cost.red));

        if (paid_mana.GetCurrent(ManaType.red) == cost.red && paid_mana.GetCurrent(ManaType.blue) == cost.blue && paid_mana.GetCurrent(ManaType.black) == cost.black
            && paid_mana.GetCurrent(ManaType.yellow) == cost.yellow && paid_mana.GetCurrent(ManaType.green) == cost.green) {
            int generic_mana = 0;
            paying_mana = true;
            tpc.SetButtonSet(ButtonSet.cancel);
            while (generic_mana < cost.generic) {
                tpc.SetWaiting("Pay Mana: " + (cost.generic - generic_mana) + " remaining");
                yield return null;
                if (p.current_action != null && p.current_action.GetType() == typeof(PayManaAction)) {
                    PayManaAction action = p.current_action as PayManaAction;
                    if (action.payer.mana_pool.GetCurrent(action.type) > 0) {
                        paid_mana.AddCurrent(action.type, 1);
                        generic_mana++;
                    }
                    yield return action.PerformAction();
                } else if (p.current_action != null && p.current_action.GetType() == typeof(CancelAction)) {
                    break;
                }
            }
            tpc.SetWaiting("Waiting for you...");
            tpc.SetButtonSet(ButtonSet.none);
            paying_mana = false;
            if (generic_mana == cost.generic) {
                paid_for = true;
            }
        }
        if (first_time) {
            mana_pointer.enabled = false;
        }
        if (paid_for && cost.generic != 0) {
            first_time = false;
        }
        callback(paid_for, paid_mana);
    }

    public static List<ITargetable> StillLegalTargets(List<ITargetable> targets, CanTargetDelegate can_target, ITargets targeter) {
        for (int i = 0; i < targets.Count; i++) {
            if (!targets[i].CanBeTargeted(targeter) || !can_target(targets[i])) {
                targets.RemoveAt(i);
                i--;
            }
        }
        return targets;
    }

    public class TryPlaySpell : PlayerAction {
        Spell play;
        bool need_payment;
        public TryPlaySpell(Spell to_play, bool need_payment = true) {
            play = to_play;
            this.need_payment = need_payment;
        }

        public override IEnumerator PerformAction() {

            if (instance.CardCanBePlayed(play, true)) {

                //Pay Mana
                bool paid_for = !need_payment;
                ManaPool paid_mana = new ManaPool();
                if (need_payment)
                    yield return instance.PlayerPayMana(play.mana_cost, play.controller, (bool b, ManaPool mp) => { paid_for = b; paid_mana = mp; });

                if (paid_for) {
                    // Pick Targets
                    bool legal_targets = true;
                    if (play.GetType() == typeof(TargetedSpell) || play.GetType().IsSubclassOf(typeof(TargetedSpell))) {
                        yield return (play as TargetedSpell).TargetRoutine((bool legal_targets_selected) => legal_targets = legal_targets_selected);
                    }

                    if (legal_targets) { // targets were picked
                        // Add Spell To Stack
                        yield return instance.AddToStack(play);
                    } else {
                        if (need_payment)
                            play.controller.mana_pool.AddCurrent(paid_mana);
                    }
                } else {
                    if (need_payment)
                        play.controller.mana_pool.AddCurrent(paid_mana);
                }

            }
        }
    }

    public class TryPlayCard : PlayerAction {
        PlayableCard play;
        public TryPlayCard(PlayableCard to_play) {
            play = to_play;
        }

        public override IEnumerator PerformAction() {
            if (instance.CardCanBePlayed(play, true) && play.CanBePlayed()) {
                if (play.GetType() == typeof(Mana)) {
                    instance.has_played_mana = true;
                }
                yield return play.OnPlay();
            }
        }
    }
}

public class PlayerAction {
    public virtual IEnumerator PerformAction() {
        yield return null;
    }
}
public class PayManaAction : PlayerAction {
    public ManaType type {
        get; private set;
    }
    public Player payer {
        get; private set;
    }

    public PayManaAction(Player p, ManaType t) {
        type = t;
        payer = p;
    }

    public override IEnumerator PerformAction() {
        yield return null;
        if (GameManager.instance.paying_mana) {
            payer.mana_pool.SubtractCurrent(type, 1);
        }
    }
}
public class PassPriority : PlayerAction {
    public override IEnumerator PerformAction() {
        yield return null;
    }
}
public class ConfirmAction : PlayerAction {
    public ConfirmAction() {
    }
    public override IEnumerator PerformAction() {
        yield return null;
    }
}
public class CancelAction : PlayerAction {
    public override IEnumerator PerformAction() {
        yield return null;
    }
}

public class FightManager {
    public Dictionary<IAttacker, Fight> attackers_to_fights;
    public List<Fight> fights;

    public FightManager() {
        attackers_to_fights = new Dictionary<IAttacker, Fight>();
        fights = new List<Fight>();
    }

    public void NewFight(IAttacker attacker, ICombatant attacked) {
        attackers_to_fights.Add(attacker, new Fight(attacker, attacked));
        fights.Add(attackers_to_fights[attacker]);
    }

    public void DeclareBlocker(IBlocker blocker, IAttacker attacker) {
        if (attackers_to_fights.ContainsKey(attacker)) {
            attackers_to_fights[attacker].AddBlocker(blocker);
        }
    }
    public void RemoveBlocker(ICombatant blocker) {
        foreach (Fight f in fights) {
            f.RemoveBlocker(blocker);
        }
    }
    public bool AssignedAsBlocker(ICombatant check) {
        foreach (Fight f in fights) {
            if (f.blockers.Contains(check)) {
                return true;
            }
        }
        return false;
    }
    public bool BeingAttacked(ICombatant check) {
        foreach (Fight f in fights) {
            if (f.attacked == check) {
                return true;
            }
        }
        return false;
    }
    public bool IsTarget(ICombatant c) {
        foreach (Fight f in fights) {
            if (c == f.attacked) {
                return true;
            }
        }
        return false;
    }
    public List<Fight> BlockerInFights(IBlocker b) {
        List<Fight> ret = new List<Fight>();
        foreach (Fight f in fights) {
            if (f.blockers.Contains(b)) {
                ret.Add(f);
            }
        }
        return ret;
    }
    public List<Fight> InvolvedInFights(ICombatant c, bool attacker, bool blocker, bool target) {
        List<Fight> ret = new List<Fight>();
        IAttacker a = c as IAttacker;
        if (attacker && attackers_to_fights.ContainsKey(a)) {
            ret.Add(attackers_to_fights[a]);
        }
        if (target) {
            foreach (Fight f in fights) {
                if (f.attacked == c) {
                    ret.Add(f);
                }
            }
        }
        if (blocker) {
            foreach (Fight f in fights) {
                if (f.blockers.Contains(c)) {
                    ret.Add(f);
                }
            }
        }

        return ret;
    }

    public HashSet<IBlocker> Blockers() {
        HashSet<IBlocker> ret = new HashSet<IBlocker>();
        foreach (Fight f in fights) {
            foreach (IBlocker b in f.blockers) {
                ret.Add(b);
            }
        }
        return ret;
    }

    public void RemoveFight(IAttacker attacker) {
        if (attackers_to_fights.ContainsKey(attacker)) {
            attackers_to_fights[attacker].EndFight();
            fights.Remove(attackers_to_fights[attacker]);
            attackers_to_fights.Remove(attacker);
        }
    }
    public void RemoveCombatantFromAllFights(ICombatant combatant) {
        IAttacker attacker = combatant as IAttacker;
        if (attacker != null && attackers_to_fights.ContainsKey(attacker)) {
            fights.Remove(attackers_to_fights[attacker]);
            attackers_to_fights.Remove(attacker);
        } else {
            foreach (Fight f in fights) {
                if (f.blockers.Contains(combatant)) {
                    f.RemoveBlocker(combatant);
                }
                if (f.attacked == combatant) {
                    f.ClearAttacked();
                }
            }
        }
    }

    public bool HasFights() {
        return fights.Count > 0;
    }

    public void SetBlocked() {
        foreach (Fight f in fights) {
            f.SetBlocked();
        }
    }

    public void Clear() {
        foreach (Fight f in fights) {
            f.EndFight();
        }
        attackers_to_fights.Clear();
        fights.Clear();
        // Here as a band-aid fix discover why targeting icons sometimes dont go away.
        foreach (TargetingDisplay td in GameObject.FindObjectsOfType<TargetingDisplay>()) {
            td.Clear();
        }
    }
}

public class Fight {
    public IAttacker attacker {
        get; private set;
    }
    public ICombatant attacked {
        get; private set;
    }
    List<ICombatant> _blockers;
    List<IBlocker> ordered_blockers;
    public bool attacker_was_blocked {
        get; private set;
    }
    public ReadOnlyCollection<ICombatant> blockers {
        get { return new ReadOnlyCollection<ICombatant>(_blockers); }
    }

    TargetingDisplay targeting_icons;

    public Fight(IAttacker attacker, ICombatant attacked) {
        this.attacker = attacker;
        this.attacked = attacked;
        _blockers = new List<ICombatant>();
        ordered_blockers = new List<IBlocker>();
        targeting_icons = GameObject.Instantiate(GameManager.instance.targeting_object).GetComponent<TargetingDisplay>();
        targeting_icons.SetActive(true);
        targeting_icons.SetAttacker(attacker.gameObject);
        targeting_icons.AddTarget(attacked.gameObject);
    }

    public void AddBlocker(ICombatant c) {
        _blockers.Add(c);
        targeting_icons.AddBlocker(c.gameObject);
    }
    public void RemoveBlocker(ICombatant c) {
        _blockers.Remove(c);
        targeting_icons.RemoveBlocker(c.gameObject);
    }

    public void SetBlocked() {
        attacker_was_blocked = _blockers.Count > 0;
    }
    public void SetOrderedBlockers(List<IBlocker> ordered) {
        foreach (IBlocker b in blockers) {
            if (ordered.Contains(b)) {
                targeting_icons.SetBlockerNumber(b.gameObject, ordered.IndexOf(b) + 1);
            } else {
                targeting_icons.SetBlockerNumber(b.gameObject, 0);
            }
        }
        ordered_blockers = ordered;
    }
    public List<IBlocker> GetOrderedBlockers() {
        if (blockers.Count == 1) {
            return new List<IBlocker>() { (IBlocker)blockers[0] };
        }
        return ordered_blockers;
    }

    public void ClearAttacked() {
        attacked = null;
        targeting_icons.Clear(false);
    }
    
    public void HighlightFight(bool is_highlighted) {
        targeting_icons.SetActive(is_highlighted);
    }
    public void EndFight() {
        targeting_icons.Clear();
    }
}
