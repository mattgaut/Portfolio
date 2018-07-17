using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Enemy))]
public class EnemyDisplay : MonoBehaviour {

    [SerializeField] Slider health_bar;
    [SerializeField] Canvas canvas;
    [SerializeField] Text healthbar_numbers;
    [SerializeField] bool always_on;
    Enemy displayed_enemy;
    float current_value = 0;

    private void Awake() {
        displayed_enemy = GetComponent<Enemy>();
        if (!always_on) health_bar.gameObject.SetActive(false);
    }

    private void Update() {
        if (current_value != displayed_enemy.health.current / displayed_enemy.health) {
            UpdateHealthBar();
        }
    }
    
    void UpdateHealthBar() {
        current_value = displayed_enemy.health.current / displayed_enemy.health;
        if (healthbar_numbers != null) {
            healthbar_numbers.text = displayed_enemy.health.current + " / " + (float)displayed_enemy.health;
        }
        health_bar.value = current_value;
        if (always_on || health_bar.value != 1) {
            health_bar.gameObject.SetActive(true);
        } else {
            health_bar.gameObject.SetActive(false);
        }
    }

    public void Enable(bool enabled) {
        canvas.gameObject.SetActive(enabled);
    }
}
