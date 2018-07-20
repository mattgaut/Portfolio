using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySkill : MonoBehaviour {

    [SerializeField]
    Text skill_name, description, cooldown;

	public void Set(string name, string desc, string cool) {
        skill_name.text = name;
        description.text = desc;
        cooldown.text = cool;
	}
    public void Set(DisplayInfo skill) {
        skill_name.text = skill.name;
        description.text = skill.desc;
        cooldown.text = skill.cool;
    }
}

public class DisplayInfo {
    public virtual string name {
        get; set;
    }
    public virtual string desc {
        get; set;
    }
    public virtual string cool {
        get; set;
    }
}
public class DisplayInfoDelegate : DisplayInfo {
    public delegate string Display();
    public Display name_del, desc_del, cool_del;

    public override string name {
        get {
            return name_del();
        }
    }
    public override string desc {
        get {
            return desc_del();
        }
    }
    public override string cool {
        get {
            return cool_del();
        }
    }
}
