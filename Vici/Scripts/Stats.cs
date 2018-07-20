using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stat {
    public abstract float current_unbuffed_value {
        get;
    }
    public float value {
        get { return (current_unbuffed_value + flat) * multi; }
    }

    public float flat {
        get; private set;
    }
    public float multi {
        get; private set;
    }

    HashSet<StatBuff> buffs;
    public Stat() {
        flat = 0;
        multi = 1;
        buffs = new HashSet<StatBuff>();
    }

    public void ApplyBuff(StatBuff buff) {
        if (!buffs.Contains(buff)) {
            flat += buff.flat_buff;
            multi *= buff.multi_buff;
            buffs.Add(buff);
        }
    }
    public void RemoveBuff(StatBuff buff) {
        if (buffs.Contains(buff)) {
            flat -= buff.flat_buff;
            multi /= buff.multi_buff;
            buffs.Remove(buff);
        }
    }

    public class StatBuff : Buff {

        public float flat_buff {
            get; private set;
        }
        public float multi_buff {
            get; private set;
        }

        Stat apply_to;
        public StatBuff(Stat apply_to, float flat, float multi) {
            this.apply_to = apply_to;
            flat_buff = flat;
            multi_buff = multi;
        }

        public override void ApplyBuff() {
            apply_to.ApplyBuff(this);
        }

        public override void RemoveBuff() {
            apply_to.RemoveBuff(this);
        }
    }
}

public class FormulaStat : Stat {

    public delegate float StatFormula();
    public StatFormula formula {
        get; protected set;
    } 

    public override float current_unbuffed_value {
        get { return formula(); }
    }

    public FormulaStat(StatFormula formula) {
        this.formula = formula;
    }
}

public class BaseStat : Stat {
    public float base_value {
        get; protected set;
    }

    public override float current_unbuffed_value {
        get {
            return base_value;
        }
    }

    public BaseStat(float base_value) : base() {
        this.base_value = base_value;
    }
}

