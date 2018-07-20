using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static UIController instance {
        get; private set;
    }

    [SerializeField]
    Slider _player_health_bar;
    public Slider player_health_bar {
        get { return _player_health_bar; }
    }

    [SerializeField]
    Slider _ability1, _ability2, _ability3, _ability4;
    public Slider ability1 {
        get { return _ability1; }
    }
    public Slider ability2 {
        get { return _ability2; }
    }
    public Slider ability3 {
        get { return _ability3; }
    }
    public Slider ability4 {
        get { return _ability4; }
    }

    [SerializeField]
    HoverField _ability1_hover, _ability2_hover, _ability3_hover, _ability4_hover;
    public HoverField ability1_hover {
        get { return _ability1_hover; }
    }
    public HoverField ability2_hover {
        get { return _ability2_hover; }
    }
    public HoverField ability3_hover {
        get { return _ability3_hover; }
    }
    public HoverField ability4_hover {
        get { return _ability4_hover; }
    }

    [SerializeField]
    Image screen_flash, fade_screen;
    [SerializeField]
    GameObject death;
    [SerializeField]
    ItemDisplay item_popup;

    [SerializeField]
    UIMap _map;
    public UIMap map {
        get { return _map; }
    }

    [SerializeField]
    UIInventory _inventory;
    public UIInventory inventory {
        get { return _inventory; }
    }

    [SerializeField]
    Slider boss_health_bar;

    [SerializeField]
    GameObject pause_screen, stats_screen;
    public bool paused {
        get; private set;
    }

    void Awake () {
		if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}

    public void FlashScreen() {
        FlashScreen(Color.red);
    }

    Coroutine flash;
    public void FlashScreen(Color c, float time = .2f) {
        if (flash != null) StopCoroutine(flash);
        flash = StartCoroutine(Flash(c, time));
    }

    IEnumerator Flash(Color c, float time) {
        float start_time = time, start_alpha = c.a;
        screen_flash.enabled = true;
        screen_flash.color = c;
        while (time > 0) {
            time -= Time.deltaTime;
            screen_flash.color = new Color(c.r,c.b,c.g, c.a * time/start_time);
            yield return null;
        }
        screen_flash.enabled = false;
        flash = null;
    }

    public void TogglePause() {
        if (!paused) {
            Pause();
        } else {
            UnPause();
        }
    }
    public void TogglePause(PlayerCharacter pc) {
        if (!paused) {
            Pause(pc);
        } else {
            UnPause();
        }
    }

    void Pause() {
        paused = true;
        pause_screen.SetActive(true);
        stats_screen.SetActive(false);
        Time.timeScale = 0f;
    }

    void Pause(PlayerCharacter pc) {
        Pause();
        stats_screen.SetActive(true);
        stats_screen.GetComponent<UIDisplayStats>().DisplayStats(pc);
    }

    void UnPause() {
        stats_screen.SetActive(false);
        pause_screen.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public void DisplayBossHealthBar(Character c) {
        StartCoroutine(UseBossBar(c));
    }

    IEnumerator UseBossBar(Character c) {
        boss_health_bar.gameObject.SetActive(true);
        while (!c.dead) {
            yield return null;

            boss_health_bar.SetFill(c.health, c.max_health.value);
        }
        boss_health_bar.gameObject.SetActive(false);
    }

    public void ShowItemPopup(Item item) {
        item_popup.DisplayItem(item);
    }

    public void ShowDeathScreen() {
        death.SetActive(true);
    }

    public IEnumerator FadeOut(float length) {
        fade_screen.enabled = true;

        fade_screen.color = new Color(0, 0, 0, 0);

        float fade_out_time = length;
        length = fade_out_time;

        while (length > 0) {
            length -= Time.deltaTime;

            fade_screen.color = new Color(0,0,0,1f - length/fade_out_time);

            yield return null;
        }
    }
    public IEnumerator FadeIn(float length) {
        fade_screen.enabled = true;

        fade_screen.color = new Color(0, 0, 0, 0);

        float fade_out_time = length;
        length = fade_out_time;

        while (length > 0) {
            length -= Time.deltaTime;

            fade_screen.color = new Color(0, 0, 0, length / fade_out_time);

            yield return null;
        }

        fade_screen.enabled = false;
    }
}
