using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeywordAbilities {

    [SerializeField]
    bool base_flying, base_first_strike, base_lifelink, base_haste, base_unblockable, base_defender, base_deathtouch;

    Dictionary<KeywordAbility, List<EffectHolder.KeywordBuff>> buffs;

    public KeywordAbilities() {
        buffs = new Dictionary<KeywordAbility, List<EffectHolder.KeywordBuff>>();

        foreach (KeywordAbility k in System.Enum.GetValues(typeof(KeywordAbility))) {
            buffs.Add(k, new List<EffectHolder.KeywordBuff>());
        }
    }

    public bool flying {
        get { return base_flying || CheckBuff(KeywordAbility.flying); }
    }
    public bool first_strike {
        get { return base_first_strike || CheckBuff(KeywordAbility.first_strike); }
    }
    public bool lifelink {
        get { return base_lifelink || CheckBuff(KeywordAbility.lifelink); }
    }
    public bool haste {
        get { return base_haste || CheckBuff(KeywordAbility.haste); }
    }
    public bool unblockable {
        get { return base_unblockable || CheckBuff(KeywordAbility.unblockable); }
    }
    public bool defender {
        get { return base_defender || CheckBuff(KeywordAbility.defender); }
    }
    public bool deathtouch {
        get { return base_deathtouch || CheckBuff(KeywordAbility.deathtouch); }
    }

    public void AddKeywordBuff(EffectHolder.KeywordBuff buff) {
        if (!buffs[buff.keyword].Contains(buff))
            buffs[buff.keyword].Add(buff);
    }
    public void RemoveKeywordBuff(EffectHolder.KeywordBuff buff) {
        if (buffs[buff.keyword].Contains(buff))
            buffs[buff.keyword].Remove(buff);
    }

    bool CheckBuff(KeywordAbility keyword) {
        foreach (EffectHolder.KeywordBuff buff in buffs[keyword]) {
            return true;
        }
        return false;
    }
}
