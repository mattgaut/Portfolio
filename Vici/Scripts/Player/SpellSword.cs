using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSword : PlayerCharacter {

    [SerializeField]
    BasicAttackInfo basic_attack_info;
    [SerializeField]
    Ability1Info ability1_info;
    [SerializeField]
    Ability3Info ability3_info;
    [SerializeField]
    Ability4Info ability4_info;

    BaseStat ability1_cooldown, ability2_cooldown, ability3_cooldown, ability4_cooldown;

    protected override void LoadSkills() {
        basic_attack = new SpellSwordBasicAttack(this);

        ability1 = new SpellSwordAbility1(this);
        ability1.cooldown = ability1_cooldown;

        ability2 = new SpellSwordAbility2(this);
        ability2.cooldown = ability2_cooldown;

        ability3 = new SpellSwordAbility3(this);
        ability3.cooldown = ability3_cooldown;

        ability4 = new SpellSwordAbility4(this);
        ability4.cooldown = ability4_cooldown;

        if (UIController.instance != null) {
            ability1.slider = UIController.instance.ability1;
            ability2.slider = UIController.instance.ability2;
            ability3.slider = UIController.instance.ability3;
            ability4.slider = UIController.instance.ability4;

            UIController.instance.ability1_hover.display = ability1.display;
            UIController.instance.ability2_hover.display = ability2.display;
            UIController.instance.ability3_hover.display = ability3.display;
            UIController.instance.ability4_hover.display = ability4.display;
        }
    }
    protected override void LoadStats() {
        base.LoadStats();

        intelligence = new BaseStat(6);
        dexterity = new BaseStat(6);
        strength = new BaseStat(5);
        vitality = new BaseStat(10);

        ability1_cooldown = new BaseStat(3);
        ability2_cooldown = new BaseStat(10);
        ability3_cooldown = new BaseStat(13);
        ability4_cooldown = new BaseStat(22);

        health = max_health.value;
        speed = new FormulaStat(() => (dexterity.value/2f + 8));
    }

    protected override void UseBasicAttack() {
        basic_attack.UseAbility();
    }
    protected override void UseAbility1() {
        ability1.UseAbility();
    }

    protected override void UseAbility2() {
        ability2.UseAbility();
    }

    protected override void UseAbility3() {
        ability3.UseAbility();
    }

    protected override void UseAbility4() {
        ability4.UseAbility();
    }


    // Ability Classes
    public class SpellSwordAbility1 : Spell {
        SpellSword spellsword;
        public FormulaStat damage {
            get; private set;
        }
        public SpellSwordAbility1(SpellSword character) : base(character) {
            spellsword = character;
            damage = new FormulaStat(() => (spellsword.dexterity.value * 3 + 5));
            display = new DisplayInfoDelegate();
            ((DisplayInfoDelegate)display).name_del = () => { return "Fireball"; };
            ((DisplayInfoDelegate)display).cool_del = () => { return "" + cooldown.value + " seconds"; };
            ((DisplayInfoDelegate)display).desc_del = () => { return "Launches a fireball forward. On contact it explodes dealing <color=red>" + damage.value + "</color> <color=darkblue>[Dexterity * 3 + 5]</color> damage."; };
        }
        public override void UseAbility() {
            base.UseAbility();
            GameObject new_proj = Instantiate(spellsword.ability1_info.obj);
            new_proj.transform.rotation = spellsword.transform.rotation;
            new_proj.transform.position = spellsword.transform.position;
            new_proj.GetComponent<Attack>().SetOnHit((IDamageable a) => a.TakeDamage((int)damage.value));
        }
    }
    public class SpellSwordAbility2 : Spell {
        SpellSword spellsword;
        float length = 5;
        public SpellSwordAbility2(SpellSword character) : base(character) {
            spellsword = character;
            display = new DisplayInfoDelegate();
            ((DisplayInfoDelegate)display).name_del = () => { return "Surge"; };
            ((DisplayInfoDelegate)display).cool_del = () => { return "" + cooldown.value + " seconds"; };
            ((DisplayInfoDelegate)display).desc_del = () => { return "Power surges in you causing your basic attack to double in size and deal double damage for " + length + " seconds."; };
        }
        public override void UseAbility() {
            base.UseAbility();
            character.StartCoroutine(Ability());
        }

        IEnumerator Ability() {
            float counter = 0;
            spellsword.basic_attack_info.obj.transform.localScale *= 2;
            spellsword.basic_attack_info.obj.transform.transform.localPosition *= (1.8f);
            Stat.StatBuff b = new Stat.StatBuff(((SpellSwordBasicAttack)spellsword.basic_attack).damage, 0, 2f);
            b.ApplyBuff();
            while (counter < length) {
                counter += Time.deltaTime;
                yield return null;
            }
            b.RemoveBuff();
            spellsword.basic_attack_info.obj.transform.localScale /= 2;
            spellsword.basic_attack_info.obj.transform.transform.localPosition /= (1.8f);
        }
    }
    public class SpellSwordAbility3 : Spell {
        Dictionary<Character, Buff> applied_buffs;
        SpellSword spellsword;
        public FormulaStat damage {
            get; private set;
        }
        public SpellSwordAbility3(SpellSword character) : base(character) {
            spellsword = character;
            applied_buffs = new Dictionary<Character, Buff>();
            damage = new FormulaStat(() => (character.intelligence.value + 3));

            display = new DisplayInfoDelegate();
            ((DisplayInfoDelegate)display).name_del = () => { return "Runic Trap"; };
            ((DisplayInfoDelegate)display).cool_del = () => { return "" + cooldown.value + " seconds"; };
            ((DisplayInfoDelegate)display).desc_del = () => { return "Creates a rune on the ground damaging enemies for <color=red>" + damage.value/spellsword.ability3_info.tick_rate + "</color> <color=darkblue>[Intelligence + 3]</color> damage per second and slowing them by 50% while they remain in the field. "; };
        }
        public override void UseAbility() {
            base.UseAbility();
            GameObject new_obj = Instantiate(spellsword.ability3_info.obj, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            new_obj.transform.position += Vector3.forward * 10;
            new_obj.GetComponent<Attack>().SetOnHit((IDamageable a) => Hit(a));
            new_obj.GetComponent<Field>().SetLeave((IDamageable a) => Leave(a));
            new_obj.GetComponent<Field>().SetTickRate(spellsword.ability3_info.tick_rate);
        }


        void Hit(IDamageable hit) {
            hit.TakeDamage((int)damage.value);
            if (hit.GetType().IsSubclassOf(typeof(Character))) {
                Character c = (Character)hit;
                if (!applied_buffs.ContainsKey(c)) {
                    Stat.StatBuff new_buff = new Stat.StatBuff(c.speed, 0, 0.5f);
                    new_buff.ApplyBuff();
                    applied_buffs.Add(c, new_buff);
                }
            }
        }

        void Leave(IDamageable left) {
            if (left.GetType().IsSubclassOf(typeof(Character))) {
                Character c = (Character)left;
                if (applied_buffs.ContainsKey(c)) {
                    applied_buffs[c].RemoveBuff();
                }
            }
        }
    }
    public class SpellSwordAbility4 : Spell {
        float delay = 1f;
        SpellSword spellsword;
        public FormulaStat damage {
            get; private set;
        }
        public SpellSwordAbility4(SpellSword character) : base(character) {
            spellsword = character;
            damage = new FormulaStat(() => (character.strength.value * 2 + character.intelligence.value * 3 + 10));

            display = new DisplayInfoDelegate();
            ((DisplayInfoDelegate)display).name_del = () => { return "Summon Meteor"; };
            ((DisplayInfoDelegate)display).cool_del = () => { return "" + cooldown.value + " seconds"; };
            ((DisplayInfoDelegate)display).desc_del = () => { return "Summons a meteor that hits the ground after " + delay + " second. When it hits the ground it deals <color=red>" + damage.value + "</color> <color=darkblue>[Strength * 2 + Intelligence * 3 + 10]</color> damage and stunning enemies hit for 2 seconds. "; };
        }
        public override void UseAbility() {
            base.UseAbility();
            spellsword.StartCoroutine(Ability());
        }
        IEnumerator Ability() {
            float counter = 0;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject shadow = Instantiate(spellsword.ability4_info.shadow, pos, Quaternion.identity);
            shadow.transform.position += Vector3.forward * 10;
            shadow.transform.localScale = new Vector3(0,0,0);
            SpriteRenderer render = shadow.GetComponentInChildren<SpriteRenderer>();
            render.color -= new Color(0, 0, 0, 1);
            while (counter < delay) {
                counter += Time.deltaTime;
                render.color += new Color(0, 0, 0, Time.deltaTime / delay);
                shadow.transform.localScale = new Vector3(counter/delay, counter / delay, counter / delay);
                yield return null;
            }
            Destroy(shadow);
            GameObject new_obj = Instantiate(spellsword.ability4_info.obj, pos, Quaternion.identity);
            new_obj.transform.position += Vector3.forward * 10;
            new_obj.GetComponent<Attack>().SetOnHit(OnHit);

            yield return null;
            yield return null;
            yield return null;

            new_obj.GetComponent<Attack>().SetHitBoxEnabled(false);

            counter = 0;
            while (counter < 2f) {
                counter += Time.deltaTime;
                yield return null;
            }

            new_obj.GetComponent<FadeOutDestroy>().StartFade(1.5f);
        }
        void OnHit(IDamageable hit) {
            hit.TakeDamage((int)damage.value);
            if (hit.GetType().IsSubclassOf(typeof(Character))) {
                Character c = (Character)hit;
                DelegateBuff new_buff = new DelegateBuff(c, (Character hit_c) => hit_c.SetStunned(true), (Character hit_c) => hit_c.SetStunned(false));
                new_buff.ApplyTimedBuff(1f);
            }
        }
    }
    public class SpellSwordBasicAttack : BasicAttack {
        public FormulaStat damage {
            get; private set;
        }
        public SpellSwordBasicAttack(SpellSword character) : base(character) {
            damage = new FormulaStat(() => (character.intelligence.value/2 + 1));
            character.basic_attack_info.obj.SetActive(true);
            character.basic_attack_info.attack.SetOnHit((IDamageable a) => { a.TakeDamage((int)damage.value); });
            character.basic_attack_info.obj.SetActive(false);
        }
        public override void UseAbility() {
            character.StartCoroutine(UseBasicAttack());
        }
        protected override IEnumerator UseBasicAttack() {
            ((SpellSword)character).basic_attack_info.obj.SetActive(true);

            while (Input.GetButton("BasicAttack")) {
                yield return null;
            }
            ((SpellSword)character).basic_attack_info.obj.SetActive(false);
        }
    }
    [Serializable]
    class BasicAttackInfo {
        [SerializeField]
        public GameObject obj;
        [SerializeField]
        public MultiHit attack;
    }
    [Serializable]
    class Ability1Info {
        [SerializeField]
        public GameObject obj;
    }
    [Serializable]
    class Ability3Info {
        [SerializeField]
        public GameObject obj;
        [SerializeField]
        public float tick_rate;
    }
    [Serializable]
    class Ability4Info {
        [SerializeField]
        public GameObject obj, shadow;
    }
}
