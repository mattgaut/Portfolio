using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHolder {

    public Card c {
        get; private set;
    }
    Dictionary<EffectDuration, List<Effect>> effects;

    public EffectHolder(Card source) {
        c = source;
        effects = new Dictionary<EffectDuration, List<Effect>>();

        foreach (EffectDuration t in System.Enum.GetValues(typeof(EffectDuration))) {
            effects.Add(t, new List<Effect>());
        }
    }

    public void AddEffect(Effect e) {
        if (!effects[e.duration].Contains(e)) {
            effects[e.duration].Add(e);
            e.Apply(this);
        }
    }
    public void RemoveEffect(Effect e) {
        if (effects[e.duration].Contains(e)) {
            effects[e.duration].Remove(e);
            e.UnApply(this);
        }
    }
    public void RemoveDuration(EffectDuration dur) {
        foreach (Effect e in effects[dur]) {
            e.UnApply(this);
        }
        effects[dur].Clear();
    }

    public int attack_power {
        get; private set;
    }
    public int toughness {
        get; private set;
    }

    public abstract class Effect {

        public EffectDuration duration {
            get; private set;
        }

        public Effect(EffectDuration d) {
            duration = d;
        }

        public abstract void Apply(EffectHolder e);
        public abstract void UnApply(EffectHolder e);
    }

    public class StatBuff : Effect {

        int attack_buff;
        int toughness_buff;

        public StatBuff(EffectDuration d, int attack_buff, int toughness_buff) : base(d) {
            this.attack_buff = attack_buff;
            this.toughness_buff = toughness_buff;
        }

        public override void Apply(EffectHolder e) {
            e.attack_power += attack_buff;
            e.toughness += toughness_buff;
            e.c.UpdateDisplay();
        }

        public override void UnApply(EffectHolder e) {
            e.attack_power -= attack_buff;
            e.toughness -= toughness_buff;
            e.c.UpdateDisplay();
        }
    }

    public class KeywordBuff : Effect {

        public KeywordAbility keyword {
            get; private set;
        }

        public KeywordBuff(EffectDuration d, KeywordAbility keyword) : base(d) {
            this.keyword = keyword;
        }

        public override void Apply(EffectHolder e) {
            e.c.triggers.keywords.AddKeywordBuff(this);
            e.c.UpdateDisplay();
        }

        public override void UnApply(EffectHolder e) {
            e.c.triggers.keywords.RemoveKeywordBuff(this);
            e.c.UpdateDisplay();
        }
    }
}