using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    static UIHandler instance;

    bool info, pause;

    [SerializeField] GameObject pause_screen, pause_screen_active_selection;
    [SerializeField] GameObject info_screen;
    [SerializeField] GameObject mini_map_object;
    [SerializeField] Map mini_map;
    [SerializeField] Map info_map;
    [SerializeField] InventoryDisplay inventory_display;
    [SerializeField] InfoDisplay info_display;

    [SerializeField] Image game_over_screen, quick_fade;
    [SerializeField] Text game_over_text;
    [SerializeField] GameObject game_over_active_selection, game_over_panel;
    [SerializeField] Camera death_camera;
    [SerializeField] List<GameObject> disable_on_death;

    [SerializeField] Image crawl_bg, crawl_fade;
    [SerializeField] Text crawl_text;
    [SerializeField] float fade_length, hold_length;

    bool unpaused_this_frame, fading;
    public static bool input_active {
        get { return instance == null || (!instance.info && !instance.pause && !instance.unpaused_this_frame && !instance.fading); }
    }

    bool take_input = true;

    private void Awake() {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }
    void Update () {
        if (take_input) {
		    if (Input.GetButtonDown("Pause")) {
                TogglePause();
            }
            if (Input.GetButtonDown("InfoScreen") && !pause) {
                ToggleShowInfoScreen();
            }
        }
    }

    private void LateUpdate() {
        unpaused_this_frame = false;
    }

    public void TogglePause() {
        pause = !pause;
        if (pause) {
            Pause();
        } else {
            UnPause();
        }
    }

    void Pause() {
        Time.timeScale = 0;
        pause_screen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pause_screen_active_selection);
    }

    void UnPause() {
        Time.timeScale = info ? 0 : 1;
        pause_screen.SetActive(false);
        unpaused_this_frame = true;
    }

    void ToggleShowInfoScreen() {
        info = !info;
        if (info) {
            OpenInfo();
        } else {
            CloseInfo();
        }
    }

    void OpenInfo() {
        mini_map_object.SetActive(false);
        info_screen.SetActive(true);
        Time.timeScale = 0;
    }

    void CloseInfo() {
        mini_map_object.SetActive(true);
        info_screen.SetActive(false);
        Time.timeScale = pause ? 0 : 1;
    }
    
    public static void FocusRoom(Room r) {
        if (instance == null) return;
        instance.mini_map.FocusRoom(r);
        instance.info_map.FocusRoom(r);
    }

    public static void DisplayItem(Item i, bool display_on_screen = true) {
        if (instance == null) return;
        instance.inventory_display.AddItem(i);
        if (display_on_screen)instance.info_display.Display(i.item_name, i.item_description, i.icon, 2f, Color.white);
    }

    public static IEnumerator FadeToBlack(float fade_in_timer) {
        yield return instance.FadeOut(fade_in_timer);
    }

    IEnumerator FadeOut(float fade_in_timer) {
        float timer = 0;
        game_over_screen.enabled = true;
        fading = true;
        while (timer < fade_in_timer) {
            timer += Time.deltaTime;
            game_over_screen.color = new Color(0, 0, 0, timer / fade_in_timer);
            yield return null;
        }
        fading = false;
        game_over_screen.color = new Color(0, 0, 0, 1);
    }

    public static void GameOver() {
        if (instance == null) return;
        instance._GameOver();
    }

    void _GameOver() {
        StartCoroutine(GameOverRoutine());
        take_input = false;
        foreach (GameObject go in disable_on_death) {
            go.SetActive(false);
        }
    }

    IEnumerator GameOverRoutine() {
        death_camera.transform.position = Camera.main.transform.position;
        death_camera.gameObject.SetActive(true);
        float fade_in_timer = 0.5f, timer = 0;


        while (timer < fade_in_timer) {
            timer += Time.deltaTime;
            death_camera.transform.position = Camera.main.transform.position;
            quick_fade.color = new Color(0, 0, 0, timer / fade_in_timer);
            yield return null;
        }
        death_camera.transform.position = Camera.main.transform.position;

        fade_in_timer = 2f;
        while (fade_in_timer > 0) {
            fade_in_timer -= Time.deltaTime;
            yield return null;
        }

        fade_in_timer = 2f;
        game_over_screen.enabled = true;
        game_over_text.enabled = true;
        while (timer < fade_in_timer) {
            timer += Time.deltaTime;
            game_over_screen.color = new Color(0, 0, 0, timer / fade_in_timer);
            game_over_text.color = new Color(1, 1, 1, Mathf.Pow(timer / fade_in_timer, 5f));
            yield return null;
        }

        fade_in_timer = 0.5f;
        while (fade_in_timer > 0) {
            fade_in_timer -= Time.deltaTime;
            yield return null;
        }

        game_over_panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(game_over_active_selection);
    }

    public static Coroutine StartEndCrawl() {
        return instance ? instance.LocalStartEndCrawl() : null;
    }

    public Coroutine LocalStartEndCrawl() {
        take_input = false;
        return StartCoroutine(EndCrawl());
    }

    IEnumerator EndCrawl() {
        // Fade in
        crawl_fade.enabled = true;
        crawl_text.enabled = true;
        crawl_bg.enabled = true;
        crawl_bg.color = Color.clear;
        crawl_fade.color = Color.clear;
        float timer = fade_length;
        while (timer > 0) {
            timer -= Time.deltaTime;
            crawl_bg.color = new Color(0, 0, 0, 1f - timer / fade_length);
            crawl_text.color = Color.white - Color.black * (timer / fade_length);
            yield return null;
        }
        crawl_bg.color = Color.black;
        // Hold
        timer = 0;
        while (timer < hold_length) {
            timer += Time.deltaTime;
            yield return null;
        }
        // Fade Out
        timer = 0;
        while (timer < fade_length) {
            timer += Time.deltaTime;
            crawl_fade.color = new Color(0, 0, 0, timer / fade_length);
            yield return null;
        }
        crawl_fade.color = Color.black;
        SceneManager.LoadScene("TitleScene");
    }
}
