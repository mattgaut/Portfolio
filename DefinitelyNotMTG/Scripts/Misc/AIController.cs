using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour {

    [SerializeField]
    Player player;

    [SerializeField]
    List<GameObject> one_drops, two_drops, three_drops, four_drops, five_drops, six_drops;


    [SerializeField]
    float creature_health_weight, creature_attack_weight, player_health_weight;

    int turn_counter = 0;

    bool played_card_this_turn;

    void Start () {

        StartCoroutine(AILoop());
	}

    IEnumerator AILoop() {
        while (true) {
            if (GameManager.instance.waiting_for == player) {
                yield return AIAction();
            } else {
                yield return null;
            }
        }
    }

    IEnumerator AIAction() {
        if (GameManager.instance.targeting == true) {
            throw new System.Exception("Doesn't know how to target");
        } else if (GameManager.instance.current_turns_player == player) {
            if (GameManager.instance.current_phase == Phase.start) {
                played_card_this_turn = false;
                turn_counter++;
                yield return PassConfirm();
            } else if (GameManager.instance.current_phase == Phase.main1 && played_card_this_turn == false) {
                float rand = Random.Range(0f, 1f);
                int count = player.field.cards.Count - GameManager.instance.OtherPlayer(player).field.cards.Count;
                if (count < -3) {
                    CreateNewCreature();
                    PlayCardInHand();
                    played_card_this_turn = false;
                } else if (count > 3) {
                    played_card_this_turn = true;
                } else if (count < 0 || player.health_remaining < 15f) {
                    CreateNewCreature();
                    PlayCardInHand();
                } else {
                    if (rand < 0.7 + .15f / turn_counter) {
                        CreateNewCreature();
                        PlayCardInHand();
                    }
                    played_card_this_turn = true;
                }
                yield return null;
                yield return null;
                if (player.hand.cards.Count > 0) {
                    Card c = player.hand.cards[0];
                    player.hand.RemoveCard(c);
                    Destroy(c.gameObject);
                }
            } else if (GameManager.instance.attacking) {
                yield return EvaluateAttacks();
            } else {
                yield return PassConfirm();
            }
        } else if (GameManager.instance.blocking) {
            yield return EvaluateBlocks();
        } else if (GameManager.instance.ordering_blockers) {
            yield return OrderBlockers(GameManager.instance.ordering_for);
        } else {
            yield return PassConfirm();
        }
    }

    IEnumerator PassConfirm() {
        player.PerformAction(new PassPriority());
        do {
            yield return null;
        } while (player.current_action != null);

        player.PerformAction(new ConfirmAction());
        do {
            yield return null;
        } while (player.current_action != null);
    }

    public void CreateNewCreature() {
        if (turn_counter == 1) {
            CreateOneDrop();
        } else if (turn_counter == 2) {
            CreateTwoDrop();
        } else if (turn_counter == 3) {
            CreateThreeDrop();
        } else if (turn_counter == 4) {
            CreateFourDrop();
        } else if (turn_counter == 5) {
            CreateFiveDrop();
        } else if (turn_counter == 6) {
            CreateSixDrop();
        } else {
            int count = Random.Range(4, 7);
            if (count == 4) {
                CreateFourDrop();
            } else if (count == 5) {
                CreateFiveDrop();
            } else if (count == 6) {
                CreateSixDrop();
            }
        }
    }

    void CreateOneDrop() {
        GameObject new_card = Instantiate(one_drops[Random.Range(0, one_drops.Count)], new Vector3(30, 30, 30), Quaternion.identity);
        Card card = new_card.GetComponent<Card>();
        card.SetOwner(player);
        card.SetController(player);
        player.hand.AddCard(card);
    }
    void CreateTwoDrop() {
        GameObject new_card = Instantiate(two_drops[Random.Range(0, two_drops.Count)], new Vector3(30, 30, 30), Quaternion.identity);
        Card card = new_card.GetComponent<Card>();
        card.SetOwner(player);
        card.SetController(player);
        player.hand.AddCard(card);
    }
    void CreateThreeDrop() {
        GameObject new_card = Instantiate(three_drops[Random.Range(0, three_drops.Count)], new Vector3(30, 30, 30), Quaternion.identity);
        Card card = new_card.GetComponent<Card>();
        card.SetOwner(player);
        card.SetController(player);
        player.hand.AddCard(card);
    }
    void CreateFourDrop() {
        GameObject new_card = Instantiate(four_drops[Random.Range(0, four_drops.Count)], new Vector3(30, 30, 30), Quaternion.identity);
        Card card = new_card.GetComponent<Card>();
        card.SetOwner(player);
        card.SetController(player);
        player.hand.AddCard(card);
    }
    void CreateFiveDrop() {
        GameObject new_card = Instantiate(five_drops[Random.Range(0, five_drops.Count)], new Vector3(30, 30, 30), Quaternion.identity);
        Card card = new_card.GetComponent<Card>();
        card.SetOwner(player);
        card.SetController(player);
        player.hand.AddCard(card);
    }
    void CreateSixDrop() {
        GameObject new_card = Instantiate(six_drops[Random.Range(0, six_drops.Count)], new Vector3(30, 30, 30), Quaternion.identity);
        Card card = new_card.GetComponent<Card>();
        card.SetOwner(player);
        card.SetController(player);
        player.hand.AddCard(card);
    }


    void PlayCardInHand() {
        player.PerformAction(new GameManager.TryPlaySpell(player.hand.cards[0] as Spell, false));
        played_card_this_turn = true;
    }

    IEnumerator EvaluateAttacks() {
        AIAttackOrder attack = new AIAttackOrder(this);

        List<IAttacker> attackers_left = new List<IAttacker>(player.field.cards.OfType<IAttacker>().Where((IAttacker a) => a.CanAttack()));
        List<IBlocker> blockers_left = new List<IBlocker>(GameManager.instance.OtherPlayer(player).field.cards.OfType<IBlocker>().Where((IBlocker b) => b.CanBlock()));
        List<ICombatant> targets_left = new List<ICombatant>(GameManager.instance.OtherPlayer(player).field.cards.OfType<ICombatant>());

        attackers_left.Sort((IAttacker a, IAttacker b) => { return System.Math.Sign(Value(a) - Value(b)); });
        blockers_left.Sort((IBlocker a, IBlocker b) => { return System.Math.Sign(Value(a) - Value(b)); });
        targets_left.Sort((ICombatant a, ICombatant b) => { return System.Math.Sign(Value(a) - Value(b)); });

        // Eat
        for (int i = 0; i < targets_left.Count; i++) {
            ICombatant com = targets_left[0];
            List<IAttacker> attackers = AttackersBigger(attackers_left, com, true);
            if (attackers.Count > 0) {
                attack.AddFight(attackers[0], com);
                if (targets_left[i] as IBlocker != null && blockers_left.Contains(targets_left[i] as IBlocker))
                    blockers_left.Remove(targets_left[i] as IBlocker);
                targets_left.RemoveAt(i);
                attackers_left.Remove(attackers[0]);
            }
        }
        // Trade
        for (int i = 0; i < targets_left.Count; i++) {
            ICombatant com = targets_left[0];
            List<IAttacker> attackers = AttackersTrade(attackers_left, com);
            if (attackers.Count > 0) {
                attack.AddFight(attackers[0], com);
                if (targets_left[i] as IBlocker != null && blockers_left.Contains(targets_left[i] as IBlocker))
                    blockers_left.Remove(targets_left[i] as IBlocker);
                targets_left.RemoveAt(i);
                attackers_left.Remove(attackers[0]);
            }
        }
        if (targets_left.Count > 0 && blockers_left.Count == 0) {
            int count = targets_left[0].health_remaining;
            for (int i = 0; i < attackers_left.Count; i++) {
                IAttacker attacker = attackers_left[i];
                if (attacker.attack_power > 0 && attacker.CanAttack(targets_left[0]) && targets_left[0].CanBeAttacked(attacker)) {
                    attack.AddFight(attacker, targets_left[0]);
                    attackers_left.Remove(attacker);
                    count -= attacker.attack_power;
                    if (count < 0) {
                        targets_left.RemoveAt(0);
                        if (targets_left.Count == 0)
                            break;
                    }
                }
            }
        }
        if (targets_left.Count == 0 && blockers_left.Count == 0) {
            foreach (IAttacker attacker in attackers_left) {
                attack.AddFight(attacker, GameManager.instance.OtherPlayer(player));
            }
        }

        attack.InputOrder();

        yield return null;
    }

    IEnumerator OrderBlockers(Fight f) {
        yield return null;
        foreach (IBlocker b in f.blockers) {
            yield return null;
            GameManager.instance.TryAddOrderTarget(b);
        }

        player.PerformAction(new ConfirmAction());
    }

    IEnumerator EvaluateBlocks() {
        AIBlockOrder block = new AIBlockOrder(this);

        List<IAttacker> attackers = new List<IAttacker>(GameManager.instance.fight_manager.attackers_to_fights.Keys);
        List<IBlocker> blockers = new List<IBlocker>();
        foreach (IBlocker com in player.field.cards.OfType<IBlocker>().Where((IBlocker b) => b.CanBlock() && !GameManager.instance.fight_manager.IsTarget(b))) {
            blockers.Add(com);
        }

        Dictionary<IBlocker, int> blockers_to_attackers = new Dictionary<IBlocker, int>();
        foreach (IBlocker blocker in player.field.cards.OfType<IBlocker>()) {
            blockers_to_attackers.Add(blocker, -1);
        }

        attackers.Sort((IAttacker a, IAttacker b) => { return System.Math.Sign(Value(a) - Value(b)); });

        // BiggerBlockers
        List<IBlocker> blockers_left = new List<IBlocker>(blockers);
        List<IAttacker> attackers_left = new List<IAttacker>(attackers);
        for (int i = 0; i < attackers_left.Count; i++) {
            IAttacker attacker = attackers_left[i];
            List<IBlocker> bigger_blockers = BlockersBigger(blockers_left, attacker);
            bigger_blockers.Sort((IBlocker a, IBlocker b) => { return -System.Math.Sign(Value(a) - Value(b)); });

            int health_left = attacker.health_remaining;

            int count = 0;
            List<IBlocker> to_remove = new List<IBlocker>();
            while (health_left > 0 && count < bigger_blockers.Count) {
                if (attacker.CanBeBlocked(bigger_blockers[count]) && bigger_blockers[count].CanBlock(attacker)) {
                    health_left -= bigger_blockers[count].attack_power;
                    to_remove.Add(bigger_blockers[count]);
                }
                count++;
            }
            if (health_left <= 0) {
                attackers_left.RemoveAt(i--);
                foreach (IBlocker c in to_remove) {
                    blockers_left.Remove(c);
                    block.AddFight(c, attacker);
                }
            }
        }

        // Trade
        for (int i = 0; i < attackers_left.Count; i++) {
            IAttacker attacker = attackers_left[i];
            List<IBlocker> blockers_trade = BlockersTrade(blockers_left, attacker);
            blockers_trade.Sort((IBlocker a, IBlocker b) => { return -System.Math.Sign(Value(a) - Value(b)); });

            int count = 0;
            while (count < blockers_trade.Count) {
                if (attacker.CanBeBlocked(blockers_trade[count]) && blockers_trade[count].CanBlock(attacker)) {
                    block.AddFight(blockers_trade[count], attacker);
                    blockers_left.Remove(blockers_trade[count]);
                    blockers_trade.Remove(blockers_trade[count]);
                }
                count++;
            }
        }

        // Bounce
        for (int i = 0; i < attackers_left.Count; i++) {
            IAttacker attacker = attackers_left[i];
            List<IBlocker> bigger_blockers = BlockersBigger(blockers_left, attacker);
            bigger_blockers.Sort((IBlocker a, IBlocker b) => { return System.Math.Sign(Value(a) - Value(b)); });

            int count = 0;
            IBlocker added = null;
            while (count < bigger_blockers.Count) {
                if (attacker.CanBeBlocked(bigger_blockers[count]) && bigger_blockers[count].CanBlock(attacker)) {
                    added = bigger_blockers[count];
                    block.AddFight(bigger_blockers[count], attacker);
                    blockers_left.Remove(bigger_blockers[count]);
                    bigger_blockers.Remove(bigger_blockers[count]);
                    count--;
                }
                count++;
            }

            // Check If Can Double Block
            count = 0;
            if (added != null) {
                while (count < bigger_blockers.Count) {
                    if (attacker.CanBeBlocked(bigger_blockers[count]) && bigger_blockers[count].CanBlock(attacker)) {
                        if (BothBlockersDontDieAttackerDies(attacker, added, bigger_blockers[count])) {
                            block.AddFight(bigger_blockers[count], attacker);
                            blockers_left.Remove(bigger_blockers[count]);
                            bigger_blockers.Remove(bigger_blockers[count]);
                            count--;
                        }
                    }
                    count++;
                }
            }
        }



        // Check If Dead
        int damage_taken = 0;
        foreach (KeyValuePair<IAttacker, Fight> pair in GameManager.instance.fight_manager.attackers_to_fights) {
            if (!block.ContatinsTarget(pair.Key) && ReferenceEquals(pair.Value.attacked, player)) {
                damage_taken += pair.Key.attack_power;
            }
        }
        
        // If Dead Start Chumping
        if (damage_taken >= player.health_remaining) {
            block = new AIBlockOrder(this);
            blockers_left = new List<IBlocker>(blockers);
            attackers_left = new List<IAttacker>(attackers);
            blockers_left.Sort((IBlocker a, IBlocker b) => { return System.Math.Sign(Value(a) - Value(b)); });
            attackers_left.Sort((IAttacker a, IAttacker b) => { return System.Math.Sign(Value(a) - Value(b)); });

            // Eat
            for (int i = 0; i < attackers_left.Count; i++) {
                IAttacker attacker = attackers_left[i];
                List<IBlocker> bigger_blockers = BlockersBigger(blockers_left, attacker, true);
                bigger_blockers.Sort((IBlocker a, IBlocker b) => { return -System.Math.Sign(Value(a) - Value(b)); });
                if (bigger_blockers.Count > 0) {
                    block.AddFight(bigger_blockers[0], attacker);
                    blockers_left.Remove(bigger_blockers[0]);
                    attackers_left.RemoveAt(i--);
                }
            }

            // Bounce
            for (int i = 0; i < attackers_left.Count; i++) {
                IAttacker attacker = attackers_left[i];
                List<IBlocker> bigger_blockers = BlockersBigger(blockers_left, attacker);
                bigger_blockers.Sort((IBlocker a, IBlocker b) => { return -System.Math.Sign(Value(a) - Value(b)); });
                if (bigger_blockers.Count > 0) {
                    block.AddFight(bigger_blockers[0], attacker);
                    blockers_left.Remove(bigger_blockers[0]);
                    attackers_left.RemoveAt(i--);
                }
            }

            // Chump
            blockers_left.Sort((IBlocker a, IBlocker b) => { return -System.Math.Sign(Value(a) - Value(b)); });
            for (int i = 0; i < attackers_left.Count; i++) {
                IAttacker attacker = attackers_left[i];
                for (int j = 0; j < blockers_left.Count; j++) {
                    if (attacker.CanBeBlocked(blockers_left[j]) && blockers_left[j].CanBlock(attacker)) {
                        block.AddFight(blockers_left[j], attacker);
                        blockers_left.RemoveAt(j);
                        attackers_left.RemoveAt(i--);
                        break;
                    }
                }
            }

            //  Double Block

        }


        block.InputOrder();

        yield return null;
    }

    bool BothBlockersDontDieAttackerDies(IAttacker attacker, IBlocker b1, IBlocker b2) {
        if (attacker.card.triggers.keywords.deathtouch && attacker.attack_power >= 2) {
            return false;
        } else if (attacker.attack_power > b1.health_remaining + b2.health_remaining) {
            return (b1.attack_power + b2.attack_power > attacker.health_remaining) && b1.card.triggers.keywords.first_strike && b2.card.triggers.keywords.first_strike;
        } else {
            return b1.attack_power + b2.attack_power > attacker.health_remaining;
        }
    }

    List<IBlocker> BlockersBigger(List<IBlocker> blockers, IAttacker attacker, bool will_kill = false) {
        List<IBlocker> ret = new List<IBlocker>(blockers);
        for (int i = 0; i < ret.Count; i++) {
            if (!ret[i].CanBlock(attacker) || !attacker.CanBeBlocked(ret[i])) {
                ret.RemoveAt(i--);
            } else {
                FightOutcome outcome = ReturnOutcome(attacker, ret[i]);
                if (outcome == FightOutcome.attacker_lives || outcome == FightOutcome.trade || (will_kill && outcome != FightOutcome.blocker_lives)) {
                    ret.RemoveAt(i--);
                }
            }
        }
        return ret;
    }
    List<IBlocker> BlockersTrade(List<IBlocker> blockers, IAttacker attacker) {
        List<IBlocker> ret = new List<IBlocker>(blockers);
        for (int i = 0; i < ret.Count; i++) {
            if (!ret[i].CanBlock(attacker) || !attacker.CanBeBlocked(ret[i])) {
                ret.RemoveAt(i--);
            } else {
                FightOutcome outcome = ReturnOutcome(attacker, ret[i]);
                if (outcome != FightOutcome.trade) {
                    ret.RemoveAt(i--);
                }
            }
        }
        return ret;
    }
    FightOutcome ReturnOutcome(IAttacker attacker, IBlocker blocker) {
        if (blocker.card.triggers.keywords.first_strike && !attacker.card.triggers.keywords.first_strike) {
            if (blocker.attack_power >= attacker.health_remaining || (blocker.card.triggers.keywords.deathtouch && blocker.attack_power > 0)) {
                return FightOutcome.blocker_lives;
            } else if (attacker.attack_power >= blocker.health_remaining) {
                return FightOutcome.attacker_lives;
            } else {
                return FightOutcome.bounce;
            }
        } else if (attacker.card.triggers.keywords.first_strike && !blocker.card.triggers.keywords.first_strike) {
            if (attacker.attack_power >= blocker.health_remaining || (attacker.card.triggers.keywords.deathtouch && attacker.attack_power > 0)) {
                return FightOutcome.attacker_lives;
            } else if (blocker.attack_power >= attacker.health_remaining) {
                return FightOutcome.blocker_lives;
            } else {
                return FightOutcome.bounce;
            }
        } else {
            if (attacker.attack_power >= blocker.health_remaining || (attacker.card.triggers.keywords.deathtouch && attacker.attack_power > 0)) {
                if (blocker.attack_power >= attacker.health_remaining || (blocker.card.triggers.keywords.deathtouch && blocker.attack_power > 0)) {
                    return FightOutcome.trade;
                } else {
                    return FightOutcome.attacker_lives;
                }
            } else {
                if (blocker.attack_power >= attacker.health_remaining || (blocker.card.triggers.keywords.deathtouch && blocker.attack_power > 0)) {
                    return FightOutcome.blocker_lives;
                } else {
                    return FightOutcome.bounce;
                }
            }
        }
    }
    FightOutcome ReturnCombatantOutcome(IAttacker attacker, ICombatant combatant) {
        SpellCombatant card = combatant as SpellCombatant;
        if (card.triggers.keywords.first_strike && !attacker.card.triggers.keywords.first_strike) {
            if (combatant.attack_power >= attacker.health_remaining || (card.triggers.keywords.deathtouch && combatant.attack_power > 0)) {
                return FightOutcome.blocker_lives;
            } else if (attacker.attack_power >= combatant.health_remaining) {
                return FightOutcome.attacker_lives;
            } else {
                return FightOutcome.bounce;
            }
        } else if (attacker.card.triggers.keywords.first_strike && !card.triggers.keywords.first_strike) {
            if (attacker.attack_power >= combatant.health_remaining || (attacker.card.triggers.keywords.deathtouch && attacker.attack_power > 0)) {
                return FightOutcome.attacker_lives;
            } else if (combatant.attack_power >= attacker.health_remaining) {
                return FightOutcome.blocker_lives;
            } else {
                return FightOutcome.bounce;
            }
        } else {
            if (attacker.attack_power >= combatant.health_remaining || (attacker.card.triggers.keywords.deathtouch && attacker.attack_power > 0)) {
                if (combatant.attack_power >= attacker.health_remaining || (card.triggers.keywords.deathtouch && combatant.attack_power > 0)) {
                    return FightOutcome.trade;
                } else {
                    return FightOutcome.attacker_lives;
                }
            } else {
                if (combatant.attack_power >= attacker.health_remaining || (card.triggers.keywords.deathtouch && combatant.attack_power > 0)) {
                    return FightOutcome.blocker_lives;
                } else {
                    return FightOutcome.bounce;
                }
            }
        }
    }

    List<IAttacker> AttackersBigger(List<IAttacker> attackers, ICombatant target, bool will_kill = false) {
        List<IAttacker> ret = new List<IAttacker>(attackers);
        for (int i = 0; i < ret.Count; i++) {
            if (!ret[i].CanAttack(target) || !target.CanBeAttacked(ret[i])) {
                ret.RemoveAt(i--);
            } else {
                FightOutcome outcome = ReturnCombatantOutcome(ret[i], target);
                if (outcome == FightOutcome.blocker_lives || outcome == FightOutcome.trade || (will_kill && outcome != FightOutcome.attacker_lives)) {
                    ret.RemoveAt(i--);
                }
            }
        }
        return ret;
    }
    List<IAttacker> AttackersTrade(List<IAttacker> attackers, ICombatant target) {
        List<IAttacker> ret = new List<IAttacker>(attackers);
        for (int i = 0; i < ret.Count; i++) {
            if (!ret[i].CanAttack(target) || !target.CanBeAttacked(ret[i])) {
                ret.RemoveAt(i--);
                continue;
            }
            if (ReturnCombatantOutcome(ret[i], target) != FightOutcome.trade) {
                ret.RemoveAt(i--);
                continue;
            }
            if (Value(ret[i]) > Value(target)) {
                ret.RemoveAt(i--);
                continue;
            }
        }
        return ret;
    }


    float Value(ICombatant com) {
        if (com.GetType() == typeof(Player)) {
            return com.health_remaining * player_health_weight;
        }
        return com.health_remaining * creature_health_weight + com.attack_power * creature_attack_weight;
    }
    float Value(ICombatant com, int force_health) {
        if (force_health <= 0) {
            return 0;
        }
        if (com.GetType() == typeof(Player)) {
            return force_health * player_health_weight * (1 - (force_health / 30));
        }
        return force_health * creature_health_weight + com.attack_power * creature_attack_weight;
    }

    class AIAttackOrder {
        Dictionary<ICombatant, ICombatant> attacker_to_target;
        Dictionary<ICombatant, int> attackers;
        Dictionary<ICombatant, int> targets;

        AIController controller;

        float value;

        public AIAttackOrder(AIController controller) {
            attacker_to_target = new Dictionary<ICombatant, ICombatant>();
            attackers = new Dictionary<ICombatant, int>();
            targets = new Dictionary<ICombatant, int>();

            this.controller = controller;
        }

        public void AddFight(ICombatant attacker, ICombatant target) {
            attacker_to_target.Add(attacker, target);
        }

        public float EvaluateValue() {
            value = 0;
            foreach (ICombatant target in targets.Keys) {
                value -= controller.Value(target, targets[target]);
            }
            foreach (ICombatant attacker in attackers.Keys) {
                value += controller.Value(attacker, attackers[attacker]);
            }

            return value;
        }

        public void InputOrder() {
            foreach (IAttacker attacker in attacker_to_target.Keys) {
                GameManager.instance.StartAttackerTargeting(attacker);
                GameManager.instance.TryChooseAttackerTarget(attacker_to_target[attacker]);
            }

            controller.player.PerformAction(new ConfirmAction());
        }

    }

    class AIBlockOrder {
        Dictionary<IBlocker, IAttacker> blockers_to_targets;
        Dictionary<ICombatant, int> blockers;
        Dictionary<ICombatant, int> targets;

        AIController controller;

        public float value;

        public AIBlockOrder(AIController controller) {
            blockers_to_targets = new Dictionary<IBlocker, IAttacker>();
            blockers = new Dictionary<ICombatant, int>();
            targets = new Dictionary<ICombatant, int>();

            this.controller = controller;
        }

        public void AddFight(IBlocker blocker, IAttacker target) {
            blockers_to_targets.Add(blocker, target);
            blockers.Add(blocker, blocker.health_remaining);
            if (!targets.ContainsKey(target))
                targets.Add(target, target.health_remaining);
        }

        public float EvaluateValue() {
            value = 0;
            foreach (ICombatant target in targets.Keys) {
                value -= controller.Value(target, targets[target]);
            }
            foreach (ICombatant blocker in blockers.Keys) {
                value += controller.Value(blocker, blockers[blocker]);
            }

            value += controller.Value(GameManager.instance.player2, GameManager.instance.player2.health_remaining);

            return value;
        }

        public bool ContatinsTarget(ICombatant target) {
            return targets.ContainsKey(target);
        }

        public void InputOrder() {
            foreach (IBlocker blocker in blockers_to_targets.Keys) {
                GameManager.instance.StartBlockerTargeting(blocker);
                GameManager.instance.TryChooseBlockerTarget(blockers_to_targets[blocker]);
            }

            controller.player.PerformAction(new ConfirmAction());
        }
    }
}