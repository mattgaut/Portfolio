using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability {

    protected PlayerCharacter character;
    public Ability (PlayerCharacter user) {
        character = user;
    }

    public DisplayInfo display {
        get; protected set;
    }
    public void SetDisplay(DisplayInfo info) {
        display = info;
    }

    public void TryUseAbility() {
        if (CanUseAbility()) UseAbility();
    }
    public virtual bool CanUseAbility() {
        return true;
    }
    public abstract void UseAbility();
}

public abstract class BasicAttack : Ability {
    public BasicAttack(PlayerCharacter user) : base(user) { }

    public override void UseAbility() {
        character.StartCoroutine(UseBasicAttack());
    }

    protected abstract IEnumerator UseBasicAttack();
}

public abstract class Spell : Ability {
    public Spell(PlayerCharacter user) : base(user) { cooldown_timer = 0; }

    public Slider slider;

    public Stat cooldown;
    protected float cooldown_timer;
    public bool on_cooldown {
        get { return cooldown_timer > 0; }
    }

    public override bool CanUseAbility() {
        return !on_cooldown;
    }
    public override void UseAbility() {
        StartCooldown();
    }

    protected void StartCooldown() {
        if (!on_cooldown) {
            character.StartCoroutine(CooldownTimer());
        }
    }
    protected IEnumerator CooldownTimer() {
        cooldown_timer = cooldown.value;
        while (cooldown_timer > 0) {
            yield return null;
            cooldown_timer -= Time.deltaTime;
            if (slider != null) {
                slider.SetFill((cooldown_timer/cooldown.value));
            }
        }
        cooldown_timer = 0;
    }
}
