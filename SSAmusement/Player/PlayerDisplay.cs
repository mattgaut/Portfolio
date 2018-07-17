using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class PlayerDisplay : MonoBehaviour {

    [SerializeField] MySlider health_bar;
    [SerializeField] GameObject buff_holder;
    [SerializeField] DisplayBuff buff_prefab;
    [SerializeField] GameObject canvas;

    Player player;

    private void Awake() {
        player = GetComponent<Player>();
    }

    public void UpdateHealthBar(float over, float under) {
        health_bar.SetFill(over, under);
    }

    public void DisplayTimedBuff(PowerUp p) {
        DisplayBuff new_display_buff = Instantiate(buff_prefab, buff_holder.transform);
        new_display_buff.SetIcon(p.icon);
        new_display_buff.SetTime(p.length);
        new_display_buff.SetBarColor(Color.green);
    }

    public void DisplayTimedBuff(Buff b) {
        DisplayBuff new_display_buff = Instantiate(buff_prefab, buff_holder.transform);
        new_display_buff.SetIcon(b.icon);
        new_display_buff.SetTime(b.buff_length);
        new_display_buff.SetBarColor(b.color);
    }

    public void Disable() {
        canvas.SetActive(false);
    }
}
