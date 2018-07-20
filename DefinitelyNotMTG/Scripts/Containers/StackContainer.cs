using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackContainer : CardContainer {

    [SerializeField]
    float card_scale, vertical_offset;
    [SerializeField]
    GameObject trigger_object;

    List<IStackEffect> effects_on_stack;

    Dictionary<IStackEffect, TargetingDisplay> targeting_icons;

    protected override void Awake() {
        base.Awake();
        effects_on_stack = new List<IStackEffect>();
        targeting_icons = new Dictionary<IStackEffect, TargetingDisplay>();
    }

    public void Add(IStackEffect effect) {
        if (effect.GetType().IsSubclassOf(typeof(Card))) {
            Card card = (Card)effect;
            effects_on_stack.Add(effect);
            AddCard(card);

            ITargets tar = effect as ITargets;
            if (tar != null) {
                GameObject new_icons = Instantiate(GameManager.instance.targeting_object);
                targeting_icons.Add(effect, new_icons.GetComponent<TargetingDisplay>());
                targeting_icons[effect].SetSource(card.gameObject);
                foreach (ITargetable t in tar.all_targets) {
                    targeting_icons[effect].AddTarget(t.gameObject);
                }
                targeting_icons[effect].SetActive(false);
            }
        } else {
            TriggeredAbility.TriggerInstance trigger = (effect as TriggeredAbility.TriggerInstance);
            GameObject new_trigger = Instantiate(trigger_object);
            new_trigger.GetComponent<DisplayTrigger>().Display(trigger);
            trigger.gameObject = new_trigger;
            effects_on_stack.Add(effect);
            UpdateView();

            ITargets tar = trigger.ability as ITargets;
            if (tar != null) {
                GameObject new_icons = Instantiate(GameManager.instance.targeting_object);
                targeting_icons.Add(effect, new_icons.GetComponent<TargetingDisplay>());
                targeting_icons[effect].SetSource(trigger.ability.source.gameObject);
                foreach (ITargetable t in tar.all_targets) {
                    targeting_icons[effect].AddTarget(t.gameObject);
                }
                targeting_icons[effect].SetActive(false);
            }
        }
    }

    public void DisplayTargetingIcons(IStackEffect effect, bool active) {
        if (targeting_icons.ContainsKey(effect)) {
            targeting_icons[effect].SetActive(active);
        }
    }

    public void Pop() {
        Remove(effects_on_stack.Count - 1);
    }

    public void Remove(int position) {
        IStackEffect effect = effects_on_stack[position];

        if (targeting_icons.ContainsKey(effect)) {
            targeting_icons[effect].Clear();
            targeting_icons.Remove(effect);
        }

        if (effect.GetType().IsSubclassOf(typeof(Card))) {
            Card card = (Card)effect;
            effects_on_stack.RemoveAt(position);
            RemoveCard(card);
        } else {
            Destroy(effects_on_stack[position].gameObject);
            effects_on_stack.RemoveAt(position);
        }
        UpdateView();
    }

    protected override void UpdateView(bool adding_card = true) {
        for (int i = 0; i < effects_on_stack.Count; i++) {
            effects_on_stack[i].gameObject.transform.position = transform.position + new Vector3(0, .02f * i, -vertical_offset * i);
            effects_on_stack[i].gameObject.transform.localScale = new Vector3(card_scale, 1, card_scale);
        }
    }
}
