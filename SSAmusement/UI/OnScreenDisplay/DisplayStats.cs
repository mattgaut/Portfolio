using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStats : MonoBehaviour {
    [SerializeField] Text health, power, speed, armor;

    public void Display(Player p) {
        health.text = p.health.current + " / " + p.health;
        power.text = p.power + "";
        speed.text = p.speed + "";
        armor.text = p.armor + "";
    }
}
