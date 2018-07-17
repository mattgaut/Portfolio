using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneralUIController : MonoBehaviour {

    public static GeneralUIController instance {
        get; private set;
    }

    [SerializeField]
    PlayerPositionUI _player_positions;
    public PlayerPositionUI player_positions { get { return _player_positions; } }
    public bool paused { get; private set; }
    bool countdown;

    [SerializeField]
    Image end_screen, start_screen;
    [SerializeField]
    Text end_text, timer_text, leaderboard_text;
    [SerializeField]
    GameObject pause_screen;
    [SerializeField]
    Button first_active, second_active;

    void Awake() {
        Time.timeScale = 1;
        if (instance == null) {
            instance = this;
            pause_screen.SetActive(true);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        pause_screen.SetActive(false);
    }

    void Update() {
        if (Input.GetButtonDown("Pause") && !countdown) {
            TogglePauseScreen();
        }
    }

    public void TogglePauseScreen() {
        if (!paused) {
            paused = true;
            GameManager.instance.sound_manager.PauseSFX();
            Time.timeScale = 0;
            pause_screen.SetActive(true);
            FindObjectOfType<EventSystem>().SetSelectedGameObject(first_active.gameObject);
        } else {
            StartCoroutine(UnpauseTimer(3));
            pause_screen.SetActive(false);
        }

    }

    IEnumerator UnpauseTimer(float length) {
        yield return CountDownRoutine(length);
    }

    public void LoadEndScreen(int number) {
        end_screen.gameObject.SetActive(true);
        end_text.text = GameManager.instance.PlayerTags.GetPlayerTag(number) + " wins!";
        FindObjectOfType<EventSystem>().SetSelectedGameObject(second_active.gameObject);

        leaderboard_text.text = "";
        List<Player> players = new List<Player>(GameManager.instance.GetPlayers());
        if (GameManager.instance.IsSurvivalMode()) {
            players.Sort((a, b) => {
                if (a.player_number == number) return 1;
                if (b.player_number == number) return -1;
                return Math.Sign(GameManager.instance.GetPlayerTime(a) - GameManager.instance.GetPlayerTime(b));
            });
        } else {
            players.Sort((a, b) => {
                if (a.player_number == number) return 1;
                if (b.player_number == number) return -1;
                return Math.Sign(GameManager.instance.GetPlayerTime(b) - GameManager.instance.GetPlayerTime(a));
            });
        }

        foreach (Player p in GameManager.instance.GetPlayers()) {
            float player_time = GameManager.instance.GetPlayerTime(p);
            if (GameManager.instance.IsSurvivalMode() && p.player_number == number) {
                leaderboard_text.text += GameManager.instance.PlayerTags.GetPlayerTag(p.player_number) + ": Survived! "+ "\n";
            } else {
                leaderboard_text.text += GameManager.instance.PlayerTags.GetPlayerTag(p.player_number) + ": " + (int)player_time / 60 + ":" + ((int)player_time % 60 + (player_time - (int)player_time)).ToString("00.##") + "\n";
            }
                
        }
    }

    public void SetColor(int player_number, Color color) {
        player_positions.SetColor(player_number, color);
    }

    public void StartInitialCountdown(float length) {
        StartCoroutine(CountDownRoutine(length));
    }

    IEnumerator CountDownRoutine(float length) {
        countdown = true;
        start_screen.enabled = true;
        timer_text.enabled = true;
        float start_length = length;
        while (length > 0) {
            timer_text.text = ((int)(length) + 1) + "";
            start_screen.color = new Color(0, 0, 0, (length / start_length) );
            length -= Time.unscaledDeltaTime;
            yield return null;
        }
        start_screen.enabled = false;
        length = 1;
        if (paused) {
            paused = false;
            GameManager.instance.sound_manager.UnPauseSFX();
            Time.timeScale = 1;
        }
        while (length > 0) {
            timer_text.text = "Go!";
            length -= Time.unscaledDeltaTime;
            yield return null;
        }
        timer_text.enabled = false;
        countdown = false;
    }
}
