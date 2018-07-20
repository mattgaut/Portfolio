using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Zone { none, exile, library, hand, field, graveyard, stack }
public enum Phase { start, main1, attackers, blockers, damage, main2, end }
public enum MajorCardType { none, sorcery, creature, instant, mana, structure }
public enum TargetContext { stackeffect, attacking, blocking, none }
public enum AbilityType { triggered, activated, local_static, static_, none }
public enum TriggerType { etb, spell_cast, dies, takes_damage, attacks, blocks, draws, eot }
public enum SelectionType { spell_cast, attacking, blocking, targeting }
public enum ButtonSet { none, pass, confirm, confirm_cancel, cancel }
public enum ManaType { red, yellow, green, blue, black }
public enum EffectDuration { eot, permanent }
public enum KeywordAbility { flying, first_strike, lifelink, haste, unblockable, deathtouch, defender }
public enum FightOutcome { bounce, attacker_lives, blocker_lives, trade }